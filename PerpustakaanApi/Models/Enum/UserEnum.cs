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

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum UserSort
        {
            [EnumMember(Value = "Id")]
            Id,
            [EnumMember(Value = "Name")]
            Name,
            [EnumMember(Value = "Email")]
            Email,
            [EnumMember(Value = "Role")]
            Role,
            [EnumMember(Value = "Gender")]
            Gender,
            [EnumMember(Value = "Date Of Birth")]
            DateOfBirth,
            [EnumMember(Value = "Phone Number")]
            PhoneNumber,
            [EnumMember(Value = "Address")]
            Address,
            [EnumMember(Value = "Date Created")]
            DateCreated,
            [EnumMember(Value = "Date Updated")]
            DateUpdated
        }
    }
}
