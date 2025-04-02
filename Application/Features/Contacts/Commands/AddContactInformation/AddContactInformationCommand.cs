using MediatR;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Domain.Helpers;
namespace Application.Features.Contacts.Commands.AddContactInformation
{
    public class AddContactInformationCommand : IRequest<Result<Guid>>
    {
        public Guid ContactId { get; set; }
        public InformationType Type { get; set; }
        public required string InformationContent { get; set; }
    }

    public class AddContactInformationCommandHandler : IRequestHandler<AddContactInformationCommand, Result<Guid>>
    {
        private readonly IPhonebookDbContext _context;

        public AddContactInformationCommandHandler(IPhonebookDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(AddContactInformationCommand request, CancellationToken cancellationToken)
        {
            var contact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.Id == request.ContactId, cancellationToken);

            if (contact == null)
            {
                return new Result<Guid>(false, "Contact not found", Guid.Empty);
            }

            var contactInfo = new ContactInformation
            {
                Id = Guid.NewGuid(),
                ContactId = request.ContactId,
                Type = request.Type,
                InformationContent = request.InformationContent
            };

            await _context.ContactInformation.AddAsync(contactInfo, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new Result<Guid>(true, "Contact information added successfully", contactInfo.Id);
        }
    }
}