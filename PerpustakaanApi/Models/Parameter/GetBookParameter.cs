namespace PerpustakaanApi.Models.Parameter
{
    public class GetBookParameter
    {
        public string Id { get; set; }
        public GetUserParameter User { get; set; }
        public string Title { get; set; }
        public GetCategoryParameter Category { get; set; }
        public List<GetGenreParameter> Genres { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Page { get; set; }
        public string Download { get; set; }
        public string Image { get; set; }
        public long ViewCount { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
