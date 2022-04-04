using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PerpustakaanApi.Models.Enum
{
    public class CategoryEnum
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum CategorySort
        {
            [EnumMember(Value = "Id")]
            Id,
            [EnumMember(Value = "Name")]
            Name,
        }
    }
}
