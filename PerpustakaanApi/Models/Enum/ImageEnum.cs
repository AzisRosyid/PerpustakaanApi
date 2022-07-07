using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PerpustakaanApi.Models.Enum
{
    public class ImageEnum
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ImageQuality
        {
            [EnumMember(Value = "High")]
            High,
            [EnumMember(Value = "Medium")]
            Medium,
            [EnumMember(Value = "Low")]
            Low,
        }
    }
}
