using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Poll.Core.Entities.Answers;
using Poll.Core.Entities.Enums;
using Poll.Core.Entities.Variants;

namespace Poll.Core.Entities;

/// <summary>
/// Сущность "Вопрос"
/// </summary>
public class Question
{
    /// <summary>
    /// Идентификатор вопроса.
    /// </summary>
    [BsonId]
    public ObjectId QuestionId { get; set; }

    /// <summary>
    /// Идентификатор страницы, на которой находится опрос.
    /// </summary>
    public string PageId { get; init; }

    /// <summary>
    /// Текст вопроса.
    /// </summary>
    public string QuestionText { get; init; }

    /// <summary>
    /// Тип вопроса.
    /// </summary>
    public QuestionType QuestionType { get; init; }

    /// <summary>
    /// Ответы на вопрос.
    /// </summary> 
    public List<QuestionAnswer> Answers { get; init; } = [];

    /// <summary>
    /// Варианты ответа.
    /// </summary>
    public List<IBaseVariants> Variants { get; init; }
}