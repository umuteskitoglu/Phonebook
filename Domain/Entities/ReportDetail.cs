using System;

namespace Domain.Entities
{
    public class ReportDetail
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ReportId { get; set; }
        public string Location { get; set; } = string.Empty;
        public int ContactCount { get; set; }
        public int PhoneNumberCount { get; set; }
    }
} 