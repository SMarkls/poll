using MongoDB.Bson.Serialization.Attributes;

namespace Poll.Core.Entities.Variants;

/// <summary>
/// Варианты ответа для вопросов типа: Списки, Группа шкал, Группа свободных ответов..
/// </summary>
[BsonNoId]
public class LinearVariants : IBaseVariants
{
    public List<string> Variants { get; init; }
}