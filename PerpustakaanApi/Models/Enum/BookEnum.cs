using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PerpustakaanApi.Models.Enum
{
    public class BookEnum
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum BookSort
        {
            [EnumMember(Value = "Popularity")]
            Popularity,
            [EnumMember(Value = "Favorite")]
            Favorite,
            [EnumMember(Value = "Id")]
            Id,
            [EnumMember(Value = "Title")]
            Title,
            [EnumMember(Value = "Author")]
            Author,
            [EnumMember(Value = "Publisher")]
            Publisher,
            [EnumMember(Value = "Total Page")]
            TotalPage,
            [EnumMember(Value = "Date Updated")]
            DateUpdated,
            [EnumMember(Value = "Date Created")]
            DateCreated
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum BookContent
        {
            [EnumMember(Value = "Full")]
            Full,
            [EnumMember(Value = "Mid")]
            Mid,
            [EnumMember(Value = "Lite")]
            Lite
        }

    }
}
