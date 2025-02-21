using FluentValidation;
using Poll.Core.Configuration.Models;

namespace Poll.Api.Models.Validation.Configuration;

public class JwtSettingsValidator : AbstractValidator<JwtSettings>
{
    public JwtSettingsValidator()
    {
        RuleFor(x => x.Audience).NotEmpty().WithMessage("Auience не может быть пустым");
        RuleFor(x => x.Issuer).NotEmpty().WithMessage("Issuer не может быть пустым");
        RuleFor(x => x.Key).NotEmpty().WithMessage("Ключ JWT не может быть пустым");
        RuleFor(x => x.MinutesLifeTime).LessThan(x => x.MinutesRefreshTokenLifeTime)
            .WithMessage("Время жизни токена доступа должно быть меньше, чем время жизни токена обновления");
    }
}