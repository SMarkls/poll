namespace Poll.Core.Entities.Answers;

/// <summary>
/// Объект передачи данных прохождения страницы опроса.
/// </summary>
public class CompletePollPageDto
{
    /// <summary>
    /// Идентификатор страницы.
    /// </summary>
    public string PageId { get; set; }

    /// <summary>
    /// Список вопросов.
    /// </summary>
    public List<CompleteQuestionDto> Questions { get; set; }
}