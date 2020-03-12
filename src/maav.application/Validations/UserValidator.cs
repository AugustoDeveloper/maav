using FluentValidation;
using MAAV.DataContracts;

namespace MAAV.Application.Validation
{
    public class UserValidator : AbstractValidator<User> 
    {
        public UserValidator()
        {
            RuleFor(u => u.Username)
                .NotNull()
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(150);

            RuleFor(u => u.Password)
                .NotNull()
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(150);

            RuleFor(u => u.FirstName)
                .NotNull()
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(150);

            RuleFor(u => u.LastName)
                .NotNull()
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(150);
        }
    }
}