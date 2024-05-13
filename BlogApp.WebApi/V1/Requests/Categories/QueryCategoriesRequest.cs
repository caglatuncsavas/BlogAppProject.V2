namespace BlogApp.WebApi.V1.Requests.Categories;

public class QueryCategoriesRequest
{
    public string? Query { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}
