using SerializationPlugin.Interfaces;
using SerializationPlugin.Serializers;

namespace SerializationPlugin;

public static class SerializerFactory
{
    public static ISerializer GetSerializer(string format)
    {
        return format.ToLower() switch
        {
            "json" => new JsonSerializer(),
            "toml" => new TomlSerializer(),
            _ => throw new ArgumentException("Unsupported format")
        };
    }
}