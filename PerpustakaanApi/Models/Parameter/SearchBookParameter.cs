using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using static PerpustakaanApi.Models.Enum.BookEnum;
using static PerpustakaanApi.Models.Enum.OrderEnum;

namespace PerpustakaanApi.Models.Parameter
{
    public class SearchBookParameter
    {
        public int Page { get; set; } = 1;
        public int Pick { get; set; } = 20;
        public string Search { get; set; } = string.Empty;
        public long? User { get; set; } = null;
        public int? Category { get; set; } = null;
        public List<int>? Genres { get; set; } = null;
        public BookStatus? Status { get; set; } = null;
        public BookSort? Sort { get; set; } = null;
        public Order? Order { get; set; } = null;
        public DateTime? Start { get; set; } = null;
        public DateTime? End { get; set; } = null;
        public BookContent? Content { get; set; } = null;
        public bool? Favorite { get; set; } = null;
        public bool Android { get; set; } = false;
    }
}
