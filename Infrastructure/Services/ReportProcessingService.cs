using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Reports.Models;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Data;

namespace Infrastructure.Services
{
    public class ReportProcessingService : IReportProcessingService
    {
        private readonly ILogger<ReportProcessingService> _logger;
        private readonly IPhonebookDbContext _dbContext;

        public ReportProcessingService(
            ILogger<ReportProcessingService> logger,
            IPhonebookDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task ProcessReportAsync(ReportProcessingMessage reportMessage)
        {
            // Get the report
            var report = await _dbContext.Reports
                .Include(r => r.Details)
                .FirstOrDefaultAsync(r => r.Id == reportMessage.ReportId);

            if (report == null)
            {
                _logger.LogWarning("Report with ID {ReportId} not found", reportMessage.ReportId);
                return;
            }

            try
            {
                var locations = _dbContext.ContactInformation
                .Where(x => x.Type == InformationType.Location)
                .GroupBy(x => x.InformationContent)
                .Select(x => new
                {
                    Location = x.Key,
                    Count = x.Count(),
                    PhoneNumberCount = x.Count(y => y.Contact.ContactInformation.Any(z => z.Type == InformationType.PhoneNumber))
                })
                .ToList();

                foreach (var location in locations)
                {
                    _dbContext.ReportDetails.Add(new ReportDetail
                    {
                        ReportId = report.Id,
                        Location = location.Location,
                        ContactCount = location.Count,
                        PhoneNumberCount = location.PhoneNumberCount
                    });
                }
                // Update report status
                report.Status = ReportStatus.Completed;

                // Save changes
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Report {ReportId} updated with {DetailCount} details",
                    report.Id, report.Details.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing report {ReportId}", report.Id);

                // Re-throw to trigger message requeuing
                throw;
            }
        }
    }
}