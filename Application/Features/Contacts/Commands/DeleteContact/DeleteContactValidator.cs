using FluentValidation;

namespace Application.Features.Contacts.Commands.DeleteContact
{
    public class DeleteContactValidator : AbstractValidator<DeleteContactCommand>
    {
        public DeleteContactValidator()
        {
            RuleFor(v => v.ContactId)
                .NotEmpty().WithMessage("Contact ID is required.");
        }
    }
} 