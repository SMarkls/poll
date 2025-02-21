using MongoDB.Bson.Serialization.Attributes;

namespace Poll.Core.Entities.Variants;

/// <summary>
/// Метка - вариантов ответа.
/// </summary>
[BsonDiscriminator(RootClass = true)]
[BsonKnownTypes(typeof(LinearVariants), typeof(MatrixVariants))]
public abstract class BaseVariants;
