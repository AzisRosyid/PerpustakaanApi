using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PerpustakaanApi.Models.Enum
{
    public class GenreEnum
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum GenreSort
        {
            [EnumMember(Value = "Id")]
            Id,
            [EnumMember(Value = "Name")]
            Name,
        }
    }
}
