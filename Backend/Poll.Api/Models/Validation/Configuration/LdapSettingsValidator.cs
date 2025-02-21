using FluentValidation;
using Poll.Core.Configuration.Models;

namespace Poll.Api.Models.Validation.Configuration;

public class LdapSettingsValidator : AbstractValidator<LdapSettings>
{
    public LdapSettingsValidator()
    {
        RuleFor(x => x.Domain).NotEmpty().WithMessage("Домен не может быть пустым");
        RuleFor(x => x.Port).NotEmpty().WithMessage("Укажите порт LDAP");
        RuleFor(x => x.Server).NotEmpty().WithMessage("Укажите сервер LDAP");
        RuleFor(x => x.ServiceAccountLogin).NotEmpty().WithMessage("Укажите логин сервисного аккаунта LDAP");
        RuleFor(x => x.ServiceAccountPassword).NotEmpty().WithMessage("Укажите пароль сервисного аккаунта LDAP");
        RuleFor(x => x.UserCatalogRDN).NotEmpty().WithMessage("Укажите каталог, содержащий сотрудников");
    }
}