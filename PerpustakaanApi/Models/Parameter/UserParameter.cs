using Microsoft.AspNetCore.Mvc.ModelBinding;
using PerpustakaanApi.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static PerpustakaanApi.Models.Enum.UserEnum;

namespace PerpustakaanApi.Models
{
    public class UserParameter
    {
        [BindRequired]
        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = null!;

        [BindRequired]
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        [BindRequired]
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        [RegularExpression("([0-9]+)")]
        public string Password { get; set; } = null!;

        [BindRequired]
        [Required]
        [EnumDataType(typeof(UserRole))]
        public UserRole Role { get; set; }

        [BindRequired]
        [Required]
        [EnumDataType(typeof(UserGender))]
        public UserGender Gender { get; set; }

        [BindRequired]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateOfBirth { get; set; }

        [BindRequired]
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; } = null!;

        [BindRequired]
        [Required]
        [StringLength(500)]
        public string Address { get; set; } = null!;

        public IFormFile? Image { get; set; } = null!;
    }
}
