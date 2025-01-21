using MongoDB.Bson.Serialization.Attributes;

namespace Poll.Core.Entities.Answers;

/// <summary>
/// Модель ответа на вопрос.
/// </summary>
[BsonNoId]
public abstract class QuestionAnswer
{
    /// <summary>
    /// Идентификатор сотрудника, давшего ответ.
    /// </summary>
    public string EmployeeId { get; init; }

    /// <summary>
    /// Имя сотрудника, давшего ответ.
    /// </summary>
    public string EmployeeName { get; init; }
}