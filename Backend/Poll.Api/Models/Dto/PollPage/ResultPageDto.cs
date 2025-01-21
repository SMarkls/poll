using Poll.Api.Models.Dto.Question;

namespace Poll.Api.Models.Dto.PollPage;

public class ResultPageDto
{
    public string PageHeader { get; set; }
    public List<QuestionResult> Results { get; set; }
}