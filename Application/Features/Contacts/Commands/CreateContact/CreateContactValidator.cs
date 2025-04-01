using FluentValidation;

namespace Application.Features.Contacts.Commands.CreateContact
{
    public class CreateContactValidator : AbstractValidator<CreateContactCommand>
    {
        public CreateContactValidator()
        {
            RuleFor(v => v.Firstname)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");
                
            RuleFor(v => v.Lastname)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");
                
            RuleFor(v => v.Company)
                .MaximumLength(200).WithMessage("Company name must not exceed 200 characters.");
        }
    }
} 