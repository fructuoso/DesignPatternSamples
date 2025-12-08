using System.Text;
using System.Text.Json;

namespace Workbench.IFormatter.Extensions;

public static class IFormatterExtensions
{
    private static readonly JsonSerializerOptions DefaultJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static byte[] Serialize<TEntity>(this TEntity entity) => entity.Serialize(DefaultJsonOptions);
    public static byte[] Serialize<TEntity>(this TEntity? entity, JsonSerializerOptions? options = null)
    {
        if (entity is null)
        {
            return [];
        }

        var json = JsonSerializer.Serialize(entity, options ?? DefaultJsonOptions);
        return Encoding.UTF8.GetBytes(json);
    }

    public static TEntity? Deserialize<TEntity>(this byte[] data) => data.Deserialize<TEntity>(DefaultJsonOptions);
    public static TEntity? Deserialize<TEntity>(this byte[]? data, JsonSerializerOptions? options = null)
    {
        if (data is null || data.Length == 0)
        {
            return default;
        }

        var json = Encoding.UTF8.GetString(data);
        return JsonSerializer.Deserialize<TEntity>(json, options ?? DefaultJsonOptions);
    }
}
