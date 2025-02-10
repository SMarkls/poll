namespace Poll.Api.Models.Dto.PollPage;

/// <summary>
/// Объект передачи данных добавления страницы опроса.
/// </summary>
public class AddPollPageDto
{
    /// <summary>
    /// Заголовок страницы
    /// </summary>
    public string PageHeader { get; init; } = "";

    /// <summary>
    /// Идентификатор опроса.
    /// </summary>
    public string PollId { get; init; }
}