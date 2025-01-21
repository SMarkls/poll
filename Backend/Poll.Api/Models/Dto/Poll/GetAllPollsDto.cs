namespace Poll.Api.Models.Dto.Poll;

public class GetAllPollsDto
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
    /// Дата и время создания опроса.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Флаг - закрыт ли опрос на данный момент.
    /// </summary>
    public bool IsOver { get; init; }
}