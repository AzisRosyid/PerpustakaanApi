using System;
using System.Collections.Generic;

namespace PerpustakaanApi.Models;

public partial class Book
{
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

    public int Status { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public virtual ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();

    public virtual Category? CategoryNavigation { get; set; }

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual User User { get; set; } = null!;
}
