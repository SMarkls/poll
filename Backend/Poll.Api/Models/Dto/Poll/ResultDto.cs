using Poll.Api.Models.Dto.PollPage;

namespace Poll.Api.Models.Dto.Poll;

public class ResultDto
{
    /// <summary>
    /// Идентифиукатор опроса
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
    /// Дата и время создания опроса.
    /// </summary>
    public DateTime CreatedAt { get; init; }

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

    /// <summary>
    /// Список идентификаторов сотрудников, прошедших опрос.
    /// </summary>
    public List<string> PassedEmployees { get; init; } = [];

    /// <summary>
    /// Ответы на вопросы.
    /// </summary>
    public List<ResultPageDto> Pages { get; set; } = [];
}