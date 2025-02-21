using FluentValidation;
using Poll.Api.Models.Dto.Rule;

namespace Poll.Api.Models.Validation.Rule;

public class ViewRuleValidator : AbstractValidator<ViewRuleDto>
{
    public ViewRuleValidator()
    {
        RuleFor(x => x.Rules).NotEmpty();
    }
}