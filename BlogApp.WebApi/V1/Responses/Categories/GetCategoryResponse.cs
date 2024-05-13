namespace BlogApp.WebApi.V1.Responses.Categories;

public class GetCategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string UrlHandle { get; set; }
}
