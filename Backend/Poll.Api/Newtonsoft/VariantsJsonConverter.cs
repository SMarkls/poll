using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Poll.Core.Entities.Variants;

namespace Poll.Api.Newtonsoft;

public class VariantsJsonConverter : JsonConverter
{
    private readonly HashSet<Type> _knownTypes = [typeof(LinearVariants), typeof(MatrixVariants)];
    private static readonly JsonSerializer _defaultSerializer = new();

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        JObject jObject = JObject.FromObject(value, _defaultSerializer);
        jObject.WriteTo(writer);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);
        var type = jsonObject["$type"]?.Value<string>();
        if (string.IsNullOrEmpty(type))
        {
            throw new JsonSerializationException($"Тип варианта ответа не указан");
        }

        var targetType = _knownTypes.FirstOrDefault(x => x.Name == type);
        if (targetType == null)
        {
            throw new JsonSerializationException("Нельзя конвертировать в этот тип");
        }

        return jsonObject.ToObject(targetType,
            JsonSerializer.Create(new JsonSerializerSettings
            {
                ContractResolver = serializer.ContractResolver,
                Converters = serializer.Converters.Where(x => x != this).ToList()
            }));
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(BaseVariants).IsAssignableFrom(objectType);
    }
}