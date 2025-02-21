using FluentValidation;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Poll.Core.Configuration.Models;

namespace Poll.Api.Models.Validation.Configuration;

public class CorsPolicyValidator : AbstractValidator<CorsPolicyConfiguration>
{
    public CorsPolicyValidator()
    {
        RuleFor(x => x.AllowedOrigins).NotEmpty().WithMessage("Список origin не может быть пустым");
    }
}