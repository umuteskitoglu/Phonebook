using MediatR;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Application.Features.Contacts.Queries.GetContactDetails
{
    public class GetContactDetailsQuery : IRequest<ContactDetailDto?>
    {
        public Guid ContactId { get; set; }
    }

    public class ContactDetailDto
    {
        public Guid Id { get; set; }
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public required string Company { get; set; }
        public List<ContactInformationDto> ContactInformation { get; set; } = new();
    }

    public class ContactInformationDto
    {
        public Guid Id { get; set; }
        public InformationType Type { get; set; }
        public required string InformationContent { get; set; }
    }

    public class GetContactDetailsQueryHandler : IRequestHandler<GetContactDetailsQuery, ContactDetailDto?>
    {
        private readonly IPhonebookDbContext _context;

        public GetContactDetailsQueryHandler(IPhonebookDbContext context)
        {
            _context = context;
        }

        public async Task<ContactDetailDto?> Handle(GetContactDetailsQuery request, CancellationToken cancellationToken)
        {
            var contact = await _context.Contacts
                .AsNoTracking()
                .Include(c => c.ContactInformation)
                .FirstOrDefaultAsync(c => c.Id == request.ContactId, cancellationToken);

            if (contact == null)
            {
                return null;
            }

            return new ContactDetailDto
            {
                Id = contact.Id,
                Firstname = contact.Firstname,
                Lastname = contact.Lastname,
                Company = contact.Company,
                ContactInformation = contact.ContactInformation.Select(ci => new ContactInformationDto
                {
                    Id = ci.Id,
                    Type = ci.Type,
                    InformationContent = ci.InformationContent
                }).ToList()
            };
        }
    }
} 