namespace Poll.Api.Models.Dto.Poll;

/// <summary>
/// Объект передачи данных обновления опроса.
/// </summary>
public class UpdatePollDto
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
    /// Идентификаторы отделов, сотрудники которых могут пройти опрос.
    /// </summary>
    public List<string>? DepartmentIds { get; init; }

    /// <summary>
    /// Список сотрудников, которые должны пройти опрос.
    /// </summary>
    public List<string>? EmployeeIds { get; init; }
}