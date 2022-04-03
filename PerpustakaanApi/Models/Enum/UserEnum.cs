using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PerpustakaanApi.Models.Enum
{
    public class UserEnum
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum UserRole
        {
            [EnumMember(Value = "Admin")]
            Admin = 0,
            [EnumMember(Value = "User")]
            User = 1
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum UserGender
        {
            [EnumMember(Value = "Male")]
            Male = 0,
            [EnumMember(Value = "Female")]
            Female = 1
        }
    }
}
