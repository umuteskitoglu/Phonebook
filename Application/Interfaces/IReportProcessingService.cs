using System;
using System.Threading.Tasks;
using Application.Features.Reports.Models;

namespace Application.Interfaces
{
    public interface IReportProcessingService
    {
        Task ProcessReportAsync(ReportProcessingMessage message);
    }
} 