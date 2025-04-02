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
    public class GetReportsQueryTests
    {
        [Fact]
        public async Task Handle_ShouldReturnAllReports_OrderedByRequestedAtDescending()
        {
            // Arrange - Setup in-memory database
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"PhonebookGetReports_DB_{Guid.NewGuid()}")
                .Options;

            // Seed the database
            var now = DateTime.UtcNow;
            using (var context = new TestDbContext(options))
            {
                context.Reports.AddRange(
                    new Report { Id = Guid.NewGuid(), RequestedAt = now.AddDays(-2), Status = ReportStatus.Completed },
                    new Report { Id = Guid.NewGuid(), RequestedAt = now, Status = ReportStatus.Preparing },
                    new Report { Id = Guid.NewGuid(), RequestedAt = now.AddDays(-1), Status = ReportStatus.Completed }
                );
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new TestDbContext(options))
            {
                var handler = new GetReportsQueryHandler(context);
                var query = new GetReportsQuery();
                var result = await handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.True(result.Success);
                Assert.Equal("Reports fetched successfully", result.Message);
                Assert.NotNull(result.Data);
                Assert.Equal(3, result.Data.Count);
                
                // Verify ordering - most recent first
                Assert.Equal(now, result.Data[0].RequestedAt);
                Assert.Equal(now.AddDays(-1), result.Data[1].RequestedAt);
                Assert.Equal(now.AddDays(-2), result.Data[2].RequestedAt);
            }
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoReportsExist()
        {
            // Arrange - Setup in-memory database
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"PhonebookGetReportsEmpty_DB_{Guid.NewGuid()}")
                .Options;

            using (var context = new TestDbContext(options))
            {
                var handler = new GetReportsQueryHandler(context);
                var query = new GetReportsQuery();

                // Act
                var result = await handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.True(result.Success);
                Assert.Equal("Reports fetched successfully", result.Message);
                Assert.NotNull(result.Data);
                Assert.Empty(result.Data);
            }
        }

        [Fact]
        public async Task Handle_ShouldHandleException_AndReturnFailureResult()
        {
            // Arrange - Setup mock that throws exception
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"PhonebookGetReportsException_DB_{Guid.NewGuid()}")
                .Options;

            using (var context = new TestDbContext(options))
            {
                // Force a null reference exception by using a null context
                var invalidContext = null as TestDbContext;
                var handler = new GetReportsQueryHandler(context);
                var query = new GetReportsQuery();
                
                // Manually replace the context with null to force an exception
                var fieldInfo = handler.GetType().GetField("_context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                fieldInfo.SetValue(handler, null);

                // Act
                var result = await handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.False(result.Success);
                Assert.Contains("Error fetching reports", result.Message);
                Assert.Null(result.Data);
            }
        }
    }
} 