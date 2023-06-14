using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using static PerpustakaanApi.Models.Enum.BookEnum;

namespace PerpustakaanApi.Models.Parameter
{
    public class BookParameter
    {

        [BindRequired]
        [Required]
        [StringLength(150)]
        public string Title { get; set; } = null!;
        
        public int? User { get; set; }

        public int? Category { get; set; }

        public List<int>? Genres { get; set; }

        public string? GenreIds { get; set; } 

        [BindRequired]
        [Required]
        [StringLength(150)]
        public string Author { get; set; } = null!;

        [BindRequired]
        [Required]
        [StringLength(150)]
        public string Publisher { get; set; } = null!;

        [BindRequired]
        [Required]
        public string Description { get; set; } = null!;

        [BindRequired]
        [Required]
        public int Page { get; set; }

        public BookStatus? Status { get; set; } 
        public IFormFile? Download { get; set; }
        public IFormFile? Image { get; set; }
    }
}
