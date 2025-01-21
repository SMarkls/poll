using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Poll.Core.Entities;

/// <summary>
/// Сущность "Страница".
/// </summary>
public class PollPage
{
    /// <summary>
    /// Идентификатор страницы
    /// </summary>
    [BsonId]
    public ObjectId PageId { get; init; }

    /// <summary>
    /// Заголовок страницы.
    /// </summary>
    public string PageHeader { get; init; } = "";

    /// <summary>
    /// Список вопросов.
    /// </summary>
    public List<Question> Questions { get; init; } = [];
}