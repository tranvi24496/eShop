using FluentValidation;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace eShop.ViewModels.System.Users
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username not null")
                .MaximumLength(50).WithMessage("Username maximum 50 character");
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Firstname is required")
                .MaximumLength(200).WithMessage("Firstname maximum is 200 character");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Lastname is required")
                .MaximumLength(200).WithMessage("Lastname maximum is 200 character");
            RuleFor(x => x.Dob).GreaterThan(DateTime.Now.AddYears(-100))
                .WithMessage("Birthday can't greater than 100 years");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Firstname is required")
                .MinimumLength(6).WithMessage("Password minimum is 200 character");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
                .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("Email format not map");
            RuleFor(x => x).Custom((request, context) => {
                if (!request.Password.Equals(request.ConfirmPassword))
                {
                    context.AddFailure("Password and PasswordConfirm is not map");
                }
            });
        }
    }
}
