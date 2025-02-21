using Poll.Api.Models.Dto.PollPage;

namespace Poll.Api.Models.Dto.Poll;

/// <summary>
/// Объект передачи данных получения опроса.
/// </summary>
public class GetPollDto
{
    /// <summary>
    /// Идентификатор опроса.
    /// </summary>
    public string PollId { get; init; }

    /// <summary>
    /// Название опроса.
    /// </summary>
    public string PollName { get; init; }

    /// <summary>
    /// Дата и время начала опроса.
    /// </summary>
    public DateTime StartTime { get; init; }

    /// <summary>
    /// Дата и время конца опроса.
    /// </summary>
    public DateTime? EndTime { get; init; }

    /// <summary>
    /// Флаг - закрыт ли опрос на данный момент.
    /// </summary>
    public bool IsOver { get; init; }

    /// <summary>
    /// Список идентификаторов страниц с вопросами.
    /// </summary>
    public List<GetPollPageDto> Pages { get; init; } = [];
}