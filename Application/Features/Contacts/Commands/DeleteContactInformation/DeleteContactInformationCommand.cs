using MediatR;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Domain.Helpers;
namespace Application.Features.Contacts.Commands.DeleteContactInformation
{
    public class DeleteContactInformationCommand : IRequest<Result>
    {
        public Guid ContactInformationId { get; set; }
    }

    public class DeleteContactInformationCommandHandler : IRequestHandler<DeleteContactInformationCommand, Result>
    {
        private readonly IPhonebookDbContext _context;

        public DeleteContactInformationCommandHandler(IPhonebookDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteContactInformationCommand request, CancellationToken cancellationToken)
        {
            var contactInfo = await _context.ContactInformation
                .FirstOrDefaultAsync(c => c.Id == request.ContactInformationId, cancellationToken);

            if (contactInfo == null)
            {
                return new Result(false, "Contact information not found");
            }

            _context.ContactInformation.Remove(contactInfo);
            await _context.SaveChangesAsync(cancellationToken);

            return new Result(true, "Contact information removed successfully");
        }
    }
} 