using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace PerpustakaanApi.Models
{
    public class LoginParameter
    {
        [BindRequired]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [BindRequired]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool? Android { get; set; } = false;
    }
}
