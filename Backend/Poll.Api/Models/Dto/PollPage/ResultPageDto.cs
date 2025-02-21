using Poll.Api.Models.Dto.Question;

namespace Poll.Api.Models.Dto.PollPage;

/// <summary>
/// Объект передачи данных получения результата опроса для страницы.
/// </summary>
public class ResultPageDto
{
    /// <summary>
    /// Заголовок страницы.
    /// </summary>
    public string PageHeader { get; set; }

    /// <summary>
    /// Ответы на вопросы.
    /// </summary>
    public List<QuestionResult> Results { get; set; }
}