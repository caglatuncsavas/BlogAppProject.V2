using BlogApp.WebApi.V1.Responses.Categories;

namespace BlogApp.WebApi.V1.Responses.BlogPosts;

public class QueryBlogPostsResponse
{
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Content { get; set; }
        public string CoverImageUrl { get; set; }
        public string Author { get; set; }
        public DateTime PublishedDate { get; set; }
        public bool IsVisible { get; set; }
        public string UrlHandle { get; set; }
        public List<CreateCategoryResponse> Categories { get; set; } = new List<CreateCategoryResponse>();
}
