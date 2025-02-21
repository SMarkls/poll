using FluentValidation;
using Poll.Api.Models.Dto.Question;

namespace Poll.Api.Models.Validation.Question;

public class AddQuestionDto : AbstractValidator<QuestionDto>
{
    public AddQuestionDto()
    {
        RuleFor(x => x.QuestionText).NotEmpty().WithMessage("Текст вопроса не может быть пустым");
    }
}