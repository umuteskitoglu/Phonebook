using System;

namespace Domain.Entities
{
    public class ContactInformation
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
        public Contact Contact { get; set; } = null!;
        public InformationType Type { get; set; }
        public string InformationContent { get; set; } = string.Empty;
    }

    public enum InformationType
    {
        PhoneNumber,
        Email,
        Location
    }
} 