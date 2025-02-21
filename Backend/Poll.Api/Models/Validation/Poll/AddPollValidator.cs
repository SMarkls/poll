using FluentValidation;
using Poll.Api.Models.Dto.Poll;

namespace Poll.Api.Models.Validation.Poll;

public class AddPollValidator : AbstractValidator<AddPollDto>
{
    public AddPollValidator()
    {
        RuleFor(x => x.PollName).NotEmpty().WithMessage("Название опроса не может быть пустым");
        RuleFor(x => x.StartTime).LessThan(x => x.EndTime)
            .WithMessage("Дата начала опроса не может быть позже даты конца опроса");
    }
}