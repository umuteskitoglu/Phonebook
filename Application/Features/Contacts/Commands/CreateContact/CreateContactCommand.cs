using MediatR;
using Application.Interfaces;
using Domain.Entities;
using Domain.Helpers;

namespace Application.Features.Contacts.Commands.CreateContact
{
    public class CreateContactCommand : IRequest<Result<Guid>>
    {
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
    }

    public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, Result<Guid>>
    {
        private readonly IPhonebookDbContext _context;

        public CreateContactCommandHandler(IPhonebookDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(CreateContactCommand request, CancellationToken cancellationToken)
        {
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                Firstname = request.Firstname,
                Lastname = request.Lastname,
                Company = request.Company
            };

            await _context.Contacts.AddAsync(contact, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new Result<Guid>(true, "Contact created successfully", contact.Id);
        }
    }
} 