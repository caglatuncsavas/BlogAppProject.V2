namespace BlogApp.WebApi.V1.Requests.BlogPosts;

public class UpdateBlogPostRequest
{
    public string Title { get; set; }
    public string ShortDescription { get; set; }
    public string Content { get; set; }
    public string CoverImageUrl { get; set; }
    public string UrlHandle { get; set; }
    public string Author { get; set; }
    public bool IsVisible { get; set; }
    public DateTime PublishedDate { get; set; }
    public List<Guid> Categories { get; set; } = new List<Guid>();
}
