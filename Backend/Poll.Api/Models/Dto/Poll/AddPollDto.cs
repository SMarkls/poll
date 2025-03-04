namespace Poll.Api.Models.Dto.Poll;

/// <summary>
/// Объект передачи данных добавления опроса.
/// </summary>
public class AddPollDto
{
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
    /// Флаг - можно ли менять ответы на вопросы после прохождения опроса.
    /// </summary>
    public bool IsAnswersEditable { get; init; }
 
    /// <summary>
    /// Идентификатор создателя опроса.
    /// </summary>
    public string? OwnerId { get; set; }

    /// <summary>
    /// Идентификаторы отделов, сотрудники которых могут пройти опрос.
    /// </summary>
    public List<string>? DepartmentIds { get; init; }

    /// <summary>
    /// Список сотрудников, которые должны пройти опрос.
    /// </summary>
    public List<string>? EmployeeIds { get; init; }
}