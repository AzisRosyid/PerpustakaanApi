using System.ComponentModel.DataAnnotations;
using static PerpustakaanApi.Models.Enum.UserEnum;

namespace PerpustakaanApi.Models.Parameter
{
    public class GetUserParameter
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        [EnumDataType(typeof(UserRole))]
        public UserRole Role { get; set; }
        [EnumDataType(typeof(UserGender))]
        public UserGender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Image { get; set; } = null!;
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
