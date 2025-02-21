using Newtonsoft.Json;

namespace Poll.Api.Newtonsoft;

public static class NewtonsoftExtension
{
    public static IMvcBuilder ConfigureSerialization(this IMvcBuilder services)
    {
        return services.AddNewtonsoftJson(opts =>
        {
            opts.SerializerSettings.TypeNameHandling = TypeNameHandling.None;
            opts.SerializerSettings.Converters.Add(new VariantsJsonConverter());
        });
    }
}