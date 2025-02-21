using FluentValidation;
using Poll.Core.Configuration.Models;

namespace Poll.Api.Models.Validation.Configuration;

public class MongoConfigurationValidator : AbstractValidator<MongoConfiguration>
{
    public MongoConfigurationValidator()
    {
        RuleFor(x => x.Database).NotEmpty().WithMessage("Название базы данных не может быть пустым");
        RuleFor(x => x.ConnectionString).NotEmpty()
            .WithMessage("Строка подключения к базе данных не может быть пустой");
    }
}