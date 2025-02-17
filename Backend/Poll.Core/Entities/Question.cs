using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Poll.Core.Entities.Answers;
using Poll.Core.Entities.Enums;
using Poll.Core.Entities.Variants;
using Poll.Core.Entities.ViewRules;

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
    /// Текст вопроса.
    /// </summary>
    public string QuestionText { get; init; }

    /// <summary>
    /// Тип вопроса.
    /// </summary>
    public QuestionType QuestionType { get; init; }

    /// <summary>
    /// Варианты ответа.
    /// </summary>
    public BaseVariants Variants { get; init; }

    /// <summary>
    /// Правило отображения вопроса.
    /// </summary>
    public ViewRule? ViewRule { get; init; } 
}