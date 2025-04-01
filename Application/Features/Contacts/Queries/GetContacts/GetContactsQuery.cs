using MediatR;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Application.Features.Contacts.Queries.GetContacts
{
    public class GetContactsQuery : IRequest<List<ContactDto>>
    {
    }

    public class ContactDto
    {
        public Guid Id { get; set; }
        public required string Firstname { get; set; } 
        public required string Lastname { get; set; }
        public required string Company { get; set; }     
    }

    public class GetContactsQueryHandler : IRequestHandler<GetContactsQuery, List<ContactDto>>
    {
        private readonly IPhonebookDbContext _context;

        public GetContactsQueryHandler(IPhonebookDbContext context)
        {
            _context = context;
        }

        public async Task<List<ContactDto>> Handle(GetContactsQuery request, CancellationToken cancellationToken)
        {
            var contacts = await _context.Contacts
                .AsNoTracking()
                .Select(c => new ContactDto
                {
                    Id = c.Id,
                    Firstname = c.Firstname,
                    Lastname = c.Lastname,
                    Company = c.Company
                })
                .ToListAsync(cancellationToken);

            return contacts;
        }
    }
} 