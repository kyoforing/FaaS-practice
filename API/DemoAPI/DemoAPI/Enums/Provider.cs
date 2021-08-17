using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DemoAPI.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Provider
    {
        Unknown = 0,
        AWS = 1,
        GCP = 2
    }
}