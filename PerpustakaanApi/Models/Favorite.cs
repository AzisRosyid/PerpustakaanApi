using System;
using System.Collections.Generic;

namespace PerpustakaanApi.Models;

public partial class Favorite
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string BookId { get; set; } = null!;

    public DateTime Date { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
