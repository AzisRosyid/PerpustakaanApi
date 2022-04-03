using System;
using System.Collections.Generic;

namespace PerpustakaanApi.Models
{
    public partial class Book
    {
        public Book()
        {
            BookGenres = new HashSet<BookGenre>();
            Favorites = new HashSet<Favorite>();
        }

        public string Id { get; set; } = null!;
        public long UserId { get; set; }
        public string Title { get; set; } = null!;
        public int? Category { get; set; }
        public string Author { get; set; } = null!;
        public string Publisher { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Page { get; set; }
        public string? Download { get; set; }
        public string? Image { get; set; }
        public long ViewCount { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public virtual Category? CategoryNavigation { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual ICollection<BookGenre> BookGenres { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }
    }
}
