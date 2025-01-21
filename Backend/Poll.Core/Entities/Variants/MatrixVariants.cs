using MongoDB.Bson.Serialization.Attributes;

namespace Poll.Core.Entities.Variants;

[BsonNoId]
public class MatrixVariants : IBaseVariants
{
    public List<string> Horizontal { get; init; }
    public List<string> Vertical { get; init; }
}