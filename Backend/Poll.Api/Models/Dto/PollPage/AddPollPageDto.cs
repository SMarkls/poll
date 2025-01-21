using Poll.Api.Models.Dto.Question;

namespace Poll.Api.Models.Dto.PollPage;

public class AddPollPageDto
{
    public string PageHeader { get; init; } = "";
    public List<QuestionDto> Questions { get; init; } = [];
    public string PollId { get; init; }
}