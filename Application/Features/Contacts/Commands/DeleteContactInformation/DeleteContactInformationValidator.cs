using FluentValidation;

namespace Application.Features.Contacts.Commands.DeleteContactInformation
{
    public class DeleteContactInformationValidator : AbstractValidator<DeleteContactInformationCommand>
    {
        public DeleteContactInformationValidator()
        {
            RuleFor(v => v.ContactInformationId)
                .NotEmpty().WithMessage("Contact information ID is required.");
        }
    }
} 