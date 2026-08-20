using System.Text.Json;
using System.Text.Json.Serialization;

namespace Falcon.FSATS.Trading.Contracts;

public static class WebContractSerialization
{
    public static JsonSerializerOptions CreateV1Options()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = false,
            WriteIndented = false
        };

        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper, allowIntegerValues: false));
        return options;
    }
}
