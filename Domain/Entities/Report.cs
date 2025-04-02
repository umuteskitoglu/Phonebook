using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Report
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public ReportStatus Status { get; set; } = ReportStatus.Preparing;
        public List<ReportDetail> Details { get; set; } = new();
    }
} 