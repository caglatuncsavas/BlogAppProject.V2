namespace BlogApp.WebApi.V1.Responses.Images;

public class QueryBlogImageResponse
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string FileExtension { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public DateTime DateCreated { get; set; }
    public List<QueryBlogImageResponse> Images { get; set; } = new List<QueryBlogImageResponse>();
}
