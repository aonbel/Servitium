namespace SerializationPlugin.Serializers.Utility;

internal static class StringUtility
{
    public static string EscapeString(string? input)
    {
        return input is null ? string.Empty : input.Replace("\"", "\\\"").Replace("\n", "\\n");
    }

    public static string UnescapeString(string input)
    {
        return input.Replace("\\\"", "\"").Replace("\\n", "\n");
    }
}