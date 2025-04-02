using MediatR;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Domain.Helpers;

namespace Application.Features.Contacts.Commands.DeleteContact
{
    public class DeleteContactCommand : IRequest<Result>
    {
        public Guid ContactId { get; set; }
    }

    public class DeleteContactCommandHandler : IRequestHandler<DeleteContactCommand, Result>
    {
        private readonly IPhonebookDbContext _context;

        public DeleteContactCommandHandler(IPhonebookDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
        {
            var contact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.Id == request.ContactId, cancellationToken);

            if (contact == null)
            {
                return new Result(false, "Contact not found");
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync(cancellationToken);

            return new Result(true, "Contact deleted successfully");
        }
    }
} 