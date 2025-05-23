using System.Text;
using SerializationPlugin.Interfaces;
using SerializationPlugin.Serializers.Utility;

namespace SerializationPlugin.Serializers;

public class TomlSerializer : ISerializer
{
    public string Serialize<T>(T obj)
    {
        if (obj == null) return "";

        var sb = new StringBuilder();
        var properties = typeof(T).GetProperties();

        for (var i = 0; i < properties.Length; i++)
        {
            var prop = properties[i];

            var value = prop.GetValue(obj);
            if (value == null) continue;

            sb.Append($"{prop.Name} = ");
            switch (value)
            {
                case string:
                    sb.Append($"\"{StringUtility.EscapeString(value.ToString())}\"");
                    break;
                case bool:
                    sb.Append(value.ToString()?.ToLower());
                    break;
                case int:
                case double:
                case float:
                    sb.Append(value);
                    break;
                default:
                    var deserializeMethod =
                        GetType().GetMethod(nameof(Serialize))!.MakeGenericMethod(prop.PropertyType);
                    var serializedObject = deserializeMethod.Invoke(this, [value]);

                    sb.Append($"[{serializedObject as string}]");
                    break;
            }

            if (i != properties.Length - 1)
            {
                sb.Append('\n');
            }
        }

        return sb.ToString();
    }

    public T? Deserialize<T>(string data)
    {
        if (string.IsNullOrWhiteSpace(data) || data == "[]")
        {
            return typeof(T).IsClass ? Activator.CreateInstance<T>() : default;
        }

        data = data.Trim();

        if (data.StartsWith('[') && data.EndsWith(']'))
        {
            data = data.Substring(1, data.Length - 2);
        }

        if (typeof(T) == typeof(string))
            return (T)(object)StringUtility.UnescapeString(data.Trim('"'));
        if (typeof(T) == typeof(int) && int.TryParse(data, out var intValue))
            return (T)(object)intValue;
        if (typeof(T) == typeof(double) && double.TryParse(data, out var doubleValue))
            return (T)(object)doubleValue;
        if (typeof(T) == typeof(bool) && bool.TryParse(data, out var boolValue))
            return (T)(object)boolValue;

        var result = Activator.CreateInstance<T>();

        var pairs = ParseTomlPairs(data);

        foreach (var prop in typeof(T).GetProperties())
        {
            if (!pairs.TryGetValue(prop.Name, out var value)) continue;

            if (string.IsNullOrWhiteSpace(value)) continue;

            var deserializeMethod = GetType().GetMethod(nameof(Deserialize))!.MakeGenericMethod(prop.PropertyType);

            var nestedObject = deserializeMethod.Invoke(this, [value])!;

            prop.SetValue(result, nestedObject);
        }

        return result;
    }

    private static Dictionary<string, string?> ParseTomlPairs(string data)
    {
        Dictionary<string, string?> result = new();
        List<string> parts = [];

        var balance = 0;
        var currentPart = string.Empty;

        foreach (var t in data)
        {
            switch (t)
            {
                case '[':
                    balance++;
                    break;
                case ']':
                    balance--;
                    break;
            }

            if (t is '\n' or '\r' && balance == 0)
            {
                if (currentPart == string.Empty) continue;

                parts.Add(currentPart);
                currentPart = string.Empty;
            }
            else
            {
                currentPart += t;
            }
        }

        if (currentPart != string.Empty)
        {
            parts.Add(currentPart);
        }

        foreach (var part in parts)
        {
            var kvp = part.Split('=', 2);

            if (kvp.Length != 2) continue;

            var key = kvp[0].Trim().Trim('"');
            var value = kvp[1].Trim();
            result[key] = value;
        }

        return result;
    }
}