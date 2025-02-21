using FluentValidation;
using Poll.Api.Models.Dto.Poll;

namespace Poll.Api.Models.Validation.Poll;

public class UpdatePollValidator : AbstractValidator<UpdatePollDto>
{
    public UpdatePollValidator()
    {
        RuleFor(x => x.PollId).NotEmpty().WithMessage("Идентификатор опроса не может быть пустым");
        RuleFor(x => x.PollName).NotEmpty().WithMessage("Название опроса не может быть пустым");
        RuleFor(x => x.StartTime).LessThan(x => x.EndTime)
            .WithMessage("Дата начала опроса не может быть позже даты конца опроса");
    }
}