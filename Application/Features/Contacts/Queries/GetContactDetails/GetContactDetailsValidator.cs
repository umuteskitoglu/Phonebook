using FluentValidation;

namespace Application.Features.Contacts.Queries.GetContactDetails
{
    public class GetContactDetailsValidator : AbstractValidator<GetContactDetailsQuery>
    {
        public GetContactDetailsValidator()
        {
            RuleFor(v => v.ContactId)
                .NotEmpty().WithMessage("Contact ID is required.");
        }
    }
} 