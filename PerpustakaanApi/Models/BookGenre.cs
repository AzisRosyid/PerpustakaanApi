using System;
using System.Collections.Generic;

namespace PerpustakaanApi.Models
{
    public partial class BookGenre
    {
        public long Id { get; set; }
        public string BookId { get; set; } = null!;
        public int GenreId { get; set; }

        public virtual Book Book { get; set; } = null!;
        public virtual Genre Genre { get; set; } = null!;
    }
}
