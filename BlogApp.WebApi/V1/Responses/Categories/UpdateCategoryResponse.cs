namespace BlogApp.WebApi.V1.Responses.Categories;

public class UpdateCategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string UrlHandle { get; set; }
}
