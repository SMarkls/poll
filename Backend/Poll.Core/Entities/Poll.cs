using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Poll.Core.Entities;

/// <summary>
/// Сущность "Опрос".
/// </summary>
public class Poll
{
    /// <summary>
    /// Идентификатор опроса.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string PollId { get; init; }

    /// <summary>
    /// Название опроса.
    /// </summary>
    public required string PollName { get; init; }

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
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Флаг - закрыт ли опрос на данный момент.
    /// </summary>
    public bool IsOver { get; init; }

    /// <summary>
    /// Флаг - можно ли менять ответы на вопросы прохождения.
    /// </summary>
    public bool IsAnswersEditable { get; init; }
 
    /// <summary>
    /// Идентификатор создателя опроса.
    /// </summary>
    public string OwnerId { get; init; }

    /// <summary>
    /// Идентификаторы отделов, сотрудники которых могут пройти опрос.
    /// </summary>
    public List<string> DepartmentIds { get; init; } = [];

    /// <summary>
    /// Список сотрудников, которые должны пройти опрос.
    /// </summary>
    public List<string> EmployeeIds { get; init; } = [];

    /// <summary>
    /// Список страниц с вопросами.
    /// </summary>
    public List<PollPage> Pages { get; init; } = [];

    /// <summary>
    /// Список идентификаторов сотрудников, прошедших опрос.
    /// </summary>
    public List<string> PassedEmployees { get; init; } = [];

    /// <summary>
    /// Ответы на вопросы.
    /// Ключ - идентификатор сотрудника.
    /// Значение - словарь, где ключ - идентификатор вопроса, значение - ответ, который дал сотрудник.
    /// </summary>
    public Dictionary<string, Dictionary<ObjectId, string>> Answers { get; set; }
}