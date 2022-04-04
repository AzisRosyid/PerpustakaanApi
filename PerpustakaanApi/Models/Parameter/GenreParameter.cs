using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace PerpustakaanApi.Models.Parameter
{
    public class GenreParameter
    {
        [BindRequired]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
