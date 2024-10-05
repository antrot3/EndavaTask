namespace ServiceLayer.Models
{
    public class ArticleEditDto
    {
        public required int? ArticleId { get; set; }
        public required string? Title { get; set; }
        public required string? Body { get; set; }
    }
}
