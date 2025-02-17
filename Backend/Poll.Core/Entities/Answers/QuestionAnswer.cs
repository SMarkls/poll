using MongoDB.Bson.Serialization.Attributes;

namespace Poll.Core.Entities.Answers;

/// <summary>
/// Модель ответа на вопрос.
/// </summary>
[BsonNoId]
public class QuestionAnswer
{
    /// <summary>
    /// Идентификатор сотрудника, давшего ответ.
    /// </summary>
    public string EmployeeId { get; init; }

    /// <summary>
    /// Значение ответа
    /// </summary>
    /// <returns></returns>
    public string Value { get; set; }
}