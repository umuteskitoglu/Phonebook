using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Contact
    {
        public Guid Id { get; set; }
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public required string Company { get; set; }
        public ICollection<ContactInformation> ContactInformation { get; set; } = new List<ContactInformation>();
    }
} 