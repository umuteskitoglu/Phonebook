using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Reports.Queries;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Phonebook.Tests.TestHelpers;
using Xunit;

namespace Phonebook.Tests.Application.Queries
{
    public class GetReportByIdQueryTests
    {
        [Fact]
        public async Task Handle_ShouldReturnReport_WhenReportExists()
        {
            // Arrange - Setup in-memory database
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"PhonebookGetReportById_DB_{Guid.NewGuid()}")
                .Options;

            // Create a report with details
            var reportId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            
            using (var context = new TestDbContext(options))
            {
                var report = new Report
                {
                    Id = reportId,
                    RequestedAt = now,
                    Status = ReportStatus.Completed,
                    Details = new List<ReportDetail>
                    {
                        new ReportDetail { Id = Guid.NewGuid(), ReportId = reportId, Location = "Istanbul", ContactCount = 10 },
                        new ReportDetail { Id = Guid.NewGuid(), ReportId = reportId, Location = "Ankara", ContactCount = 5 }
                    }
                };
                
                await context.Reports.AddAsync(report);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new TestDbContext(options))
            {
                var handler = new GetReportByIdQueryHandler(context);
                var query = new GetReportByIdQuery { Id = reportId };

                var result = await handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.True(result.Success);
                Assert.Equal("Report fetched successfully", result.Message);
                Assert.NotNull(result.Data);
                Assert.Equal(reportId, result.Data.Id);
                Assert.Equal(ReportStatus.Completed, result.Data.Status);
                Assert.Equal(now, result.Data.RequestedAt);
                
                // Check details
                Assert.NotNull(result.Data.Details);
                Assert.Equal(2, result.Data.Details.Count);
                
                // Verify detail locations and counts
                var istanbulDetail = result.Data.Details.Find(d => d.Location == "Istanbul");
                var ankaraDetail = result.Data.Details.Find(d => d.Location == "Ankara");
                
                Assert.NotNull(istanbulDetail);
                Assert.NotNull(ankaraDetail);
                Assert.Equal(10, istanbulDetail.ContactCount);
                Assert.Equal(5, ankaraDetail.ContactCount);
            }
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFoundResult_WhenReportDoesNotExist()
        {
            // Arrange - Setup in-memory database
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"PhonebookGetReportByIdNotFound_DB_{Guid.NewGuid()}")
                .Options;

            var nonExistentReportId = Guid.NewGuid();

            using (var context = new TestDbContext(options))
            {
                var handler = new GetReportByIdQueryHandler(context);
                var query = new GetReportByIdQuery { Id = nonExistentReportId };

                // Act
                var result = await handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.False(result.Success);
                Assert.Equal($"Report with ID {nonExistentReportId} not found", result.Message);
                Assert.Null(result.Data);
            }
        }

        [Fact]
        public async Task Handle_ShouldHandleException_AndReturnFailureResult()
        {
            // Arrange - Setup mock that throws exception
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"PhonebookGetReportByIdException_DB_{Guid.NewGuid()}")
                .Options;

            using (var context = new TestDbContext(options))
            {
                // Force a null reference exception by using a null context
                var handler = new GetReportByIdQueryHandler(context);
                var query = new GetReportByIdQuery { Id = Guid.NewGuid() };
                
                // Manually replace the context with null to force an exception
                var fieldInfo = handler.GetType().GetField("_context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                fieldInfo.SetValue(handler, null);

                // Act
                var result = await handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.False(result.Success);
                Assert.Contains("Error fetching report", result.Message);
                Assert.Null(result.Data);
            }
        }
    }
} 