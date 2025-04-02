using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Reports.Commands;
using Application.Features.Reports.Models;
using Application.Interfaces;
using Domain.Entities;
using Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using Phonebook.Tests.TestHelpers;
using Xunit;

namespace Phonebook.Tests.Application.Commands
{
    public class CreateReportCommandTests
    {
        [Fact]
        public async Task Handle_ShouldCreateReport_WhenValidRequest()
        {
            // Arrange - Setup in-memory database
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"PhonebookCreateReport_DB_{Guid.NewGuid()}")
                .Options;

            var mockMessageService = new Mock<IMessageService>();
            mockMessageService
                .Setup(m => m.PublishAsync(It.IsAny<string>(), It.IsAny<ReportProcessingMessage>()))
                .Returns(Task.CompletedTask);
            
            Guid reportId;

            using (var context = new TestDbContext(options))
            {
                var handler = new CreateReportCommandHandler(context, mockMessageService.Object);
                var command = new CreateReportCommand();

                // Act
                var result = await handler.Handle(command, CancellationToken.None);

                // Assert
                Assert.True(result.Success);
                Assert.Equal("Report created successfully", result.Message);
                Assert.NotEqual(Guid.Empty, result.Data);
                reportId = result.Data;
            }

            // Verify that report was created
            using (var context = new TestDbContext(options))
            {
                var report = await context.Reports.FindAsync(reportId);
                Assert.NotNull(report);
                Assert.Equal(ReportStatus.Preparing, report.Status);
                Assert.NotEqual(default, report.RequestedAt);
            }

            // Verify that message service was called
            mockMessageService.Verify(
                m => m.PublishAsync(
                    "report-processing", 
                    It.Is<ReportProcessingMessage>(msg => msg.ReportId == reportId)), 
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenMessageServiceFails()
        {
            // Arrange - Setup in-memory database
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"PhonebookCreateReportFail_DB_{Guid.NewGuid()}")
                .Options;

            var mockMessageService = new Mock<IMessageService>();
            mockMessageService
                .Setup(m => m.PublishAsync(It.IsAny<string>(), It.IsAny<ReportProcessingMessage>()))
                .ThrowsAsync(new Exception("Failed to send message"));

            using (var context = new TestDbContext(options))
            {
                var handler = new CreateReportCommandHandler(context, mockMessageService.Object);
                var command = new CreateReportCommand();

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(() => 
                    handler.Handle(command, CancellationToken.None));
            }
        }
    }
} 