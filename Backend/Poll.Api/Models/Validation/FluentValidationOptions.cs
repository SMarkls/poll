using FluentValidation;
using Microsoft.Extensions.Options;

namespace Poll.Api.Models.Validation;
public class FluentValidationOptions<T>(IValidator<T>? validator) : IValidateOptions<T>
    where T : class
{
    public ValidateOptionsResult Validate(string name, T options)
    {
        if (validator is null)
        {
            return ValidateOptionsResult.Skip;
        }

        var result = validator.Validate(options);
        if (result.IsValid) return ValidateOptionsResult.Success;

        var errors = result.Errors
            .Select(f => $"Валидация конфигурации '{f.PropertyName}' завершилась с ошибкой: {f.ErrorMessage}");
        return ValidateOptionsResult.Fail(errors);
    }
}