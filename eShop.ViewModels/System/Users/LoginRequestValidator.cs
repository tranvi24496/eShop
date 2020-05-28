using FluentValidation;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace eShop.ViewModels.System.Users
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("UserName is not null");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is not null");
        }
    }
}
