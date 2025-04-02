using System;

namespace Application.Features.Reports.Models
{
    public class ReportProcessingMessage
    {
        public Guid ReportId { get; set; }
        public DateTime RequestedAt { get; set; }
    }
} 