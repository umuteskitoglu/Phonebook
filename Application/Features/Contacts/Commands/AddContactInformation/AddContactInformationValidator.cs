using FluentValidation;
using Domain.Entities;

namespace Application.Features.Contacts.Commands.AddContactInformation
{
    public class AddContactInformationValidator : AbstractValidator<AddContactInformationCommand>
    {
        public AddContactInformationValidator()
        {
            RuleFor(v => v.ContactId)
                .NotEmpty().WithMessage("Contact ID is required.");
                
            RuleFor(v => v.Type)
                .IsInEnum().WithMessage("Invalid information type.");
                
            RuleFor(v => v.InformationContent)
                .NotEmpty().WithMessage("Information content is required.")
                .MaximumLength(500).WithMessage("Information content must not exceed 500 characters.");
                
            // Custom validation for specific information types
            When(v => v.Type == InformationType.PhoneNumber, () => {
                RuleFor(v => v.InformationContent)
                    .Matches(@"^\+?[0-9\s-()]+$").WithMessage("Invalid phone number format.");
            });
            
            When(v => v.Type == InformationType.Email, () => {
                RuleFor(v => v.InformationContent)
                    .EmailAddress().WithMessage("Invalid email address format.");
            });
        }
    }
} 