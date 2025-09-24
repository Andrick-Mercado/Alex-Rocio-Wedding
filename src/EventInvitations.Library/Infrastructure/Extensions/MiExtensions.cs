using System.Text.Json;

namespace PersonalPortfolio.Library.Infrastructure.Extensions;

public static class MiExtensions
{
    private static readonly JsonSerializerOptions CamelCaseOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static T DeserializeWithCamelCase<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json, CamelCaseOptions);
    }

    public static string Serialize<T>(this T data)
    {
        return JsonSerializer.Serialize(data);
    }

    public static string FirstCharToUpper(this string input)
    {
        if(string.IsNullOrWhiteSpace(input))
            return input;

        return string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1));
    }
}