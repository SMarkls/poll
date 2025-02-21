using FluentValidation;
using Poll.Api.Models.Dto.Rule;

namespace Poll.Api.Models.Validation.Rule;

public class RuleValidator : AbstractValidator<RuleDto>
{
    public RuleValidator()
    {
        RuleFor(x => x.Value).NotEmpty().WithMessage("Значение правила не может быть пустым");
        RuleFor(x => x.QuestionId).NotEmpty().WithMessage("Идентификатор вопроса не может быть пустым");
    }
}