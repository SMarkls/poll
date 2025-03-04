using Poll.Api.Models.Dto.Question;

namespace Poll.Api.Models.Dto.PollPage;

/// <summary>
/// Объект передачи данных получения странииы опроса.
/// </summary>
public class GetPollPageDto
{
    /// <summary>
    /// Идентификатор страницы
    /// </summary>
    public string PageId { get; init; }

    /// <summary>
    /// Заголовок страницы.
    /// </summary>
    public string PageHeader { get; init; } = "";

    /// <summary>
    /// Список идентификаторов вопросов.
    /// </summary>
    public List<GetQuestionDto> Questions { get; init; } = [];
}