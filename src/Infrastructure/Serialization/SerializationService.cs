using SerializationPlugin;

namespace Infrastructure.Serialization;

public class SerializationService
{
    public string Serialize<T>(T objectToSerialize, string format)
    {
        return SerializerFactory.GetSerializer(format).Serialize(objectToSerialize);
    }

    public T? Deserialize<T>(string data, string format)
    {
        return SerializerFactory.GetSerializer(format).Deserialize<T>(data);
    }
}