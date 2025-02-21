namespace Poll.Core.Entities.Answers;

/// <summary>
/// Объект передачи данных завершения прохождения опроса.
/// </summary>
public class CompletePollDto
{
    /// <summary>
    /// Список страниц с вопросами.
    /// </summary>
    public List<CompletePollPageDto> Pages { get; init; } = [];
}