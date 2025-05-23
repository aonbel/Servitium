namespace SerializationPlugin;

public static class FileSerializer
{
    public static void SaveToFile<T>(T obj, string filePath, string format)
    {
        var serializer = SerializerFactory.GetSerializer(format);
        var data = serializer.Serialize(obj);
        File.WriteAllText(filePath, data);
    }

    public static T? LoadFromFile<T>(string filePath, string format) where T : new()
    {
        if (!File.Exists(filePath)) return new T();
        var data = File.ReadAllText(filePath);
        var serializer = SerializerFactory.GetSerializer(format);
        return serializer.Deserialize<T>(data);
    }
}