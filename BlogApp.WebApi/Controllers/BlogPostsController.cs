using BlogApp.WebApi.Data.Entities;
using BlogApp.WebApi.DTOs;
using BlogApp.WebApi.Repositories.Implementation;
using BlogApp.WebApi.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BlogPostsController : ControllerBase
{
    private readonly IBlogPostRepository blogPostRepository;
    private readonly ICategoryRepository categoryRepository;

    public BlogPostsController(IBlogPostRepository blogPostRepository, ICategoryRepository categoryRepository)
    {
        this.blogPostRepository = blogPostRepository;
        this.categoryRepository = categoryRepository;
    }

    //POST: {apiBaseUrl}/api/blogposts
    [HttpPost]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> CreateBlogPost([FromBody]CreateBlogPostRequestDto request)
    {
        //Convert DTO to Domain Model
        var blogPost = new BlogPost
        {
            Author = request.Author,
            Content = request.Content,
            CoverImageUrl = request.CoverImageUrl,
            IsVisible = request.IsVisible,
            PublishedDate = request.PublishedDate,
            ShortDescription = request.ShortDescription,
            Title = request.Title,
            UrlHandle = request.UrlHandle,
            Categories = new List<Category>()
        };

        foreach (var categoryGuid in request.Categories)
        {
           var existingCategory = await categoryRepository.GetById(categoryGuid);
            if (existingCategory is not null)
            {
                blogPost.Categories.Add(existingCategory);
            }
        }

        //Call the repository to create the blog post
        blogPost = await blogPostRepository.CreateAsync(blogPost);

        //Before we return the response, we have to convert the domain model back to a DTO
        var response = new BlogPostDto
        {
            Id = blogPost.Id,
            Author = blogPost.Author,
            Content = blogPost.Content,
            CoverImageUrl = blogPost.CoverImageUrl,
            IsVisible = blogPost.IsVisible,
            PublishedDate = blogPost.PublishedDate,
            ShortDescription = blogPost.ShortDescription,
            Title = blogPost.Title,
            UrlHandle = blogPost.UrlHandle,
            Categories = blogPost.Categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                UrlHandle = c.UrlHandle
            }).ToList()
        };
        return Ok(response);
    }

    //GET: {apiBaseUrl}/api/blogposts
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(QueryBlogPostsResponse))]
    public async Task<IActionResult> QueryBlogPosts()
    {
        var blogPosts = await blogPostRepository.GetAllAsync();

        //Convert the domain model to DTO
        var response = new QueryBlogPostsResponse
        {
            BlogPosts = blogPosts.Select(blogPost => new BlogPostDto
            {
                Id = blogPost.Id,
                Author = blogPost.Author,
                Content = blogPost.Content,
                CoverImageUrl = blogPost.CoverImageUrl,
                IsVisible = blogPost.IsVisible,
                PublishedDate = blogPost.PublishedDate,
                ShortDescription = blogPost.ShortDescription,
                Title = blogPost.Title,
                UrlHandle = blogPost.UrlHandle,
                Categories = blogPost.Categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    UrlHandle = c.UrlHandle
                }).ToList()
            }).ToList()
        };
        return Ok(response);
        }  
    }

    //GET: {apiBaseUrl}/api/blogposts/{id}
    [HttpGet]
    [Route("{id:Guid}")]
    public async Task<IActionResult> GetBlogPostsById([FromRoute]Guid id)
    {
        //Get the BlogPost from Repo
        var blogPost = await blogPostRepository.GetByIdAsync(id);
        if (blogPost is null)
        {
            return NotFound();
        }

        //Convert the domain model to DTO

        var response = new BlogPostDto
        {
            Id = blogPost.Id,
            Author = blogPost.Author,
            Content = blogPost.Content,
            CoverImageUrl = blogPost.CoverImageUrl,
            IsVisible = blogPost.IsVisible,
            PublishedDate = blogPost.PublishedDate,
            ShortDescription = blogPost.ShortDescription,
            Title = blogPost.Title,
            UrlHandle = blogPost.UrlHandle,
            Categories = blogPost.Categories.Select(p => new CategoryDto
            {
                Id = p.Id,
                Name = p.Name,
                UrlHandle = p.UrlHandle
            }).ToList()
        };
        return Ok(response);
    }

    //GET: {apiBaseUrl}/api/blogposts/{urlHandle}
    [HttpGet]
    [Route("{urlHandle}")]
    public async Task<IActionResult> GetBlogPostByUrlHandle([FromRoute] string urlHandle)
    {
        //Get blogPost details from repository
       var blogPost = await blogPostRepository.GetByUrlHandleAsync(urlHandle);
        if(blogPost is null)
        {
            return NotFound();
        }

        var response = new BlogPostDto
        {
            Id = blogPost.Id,
            Author = blogPost.Author,
            Content = blogPost.Content,
            CoverImageUrl = blogPost.CoverImageUrl,
            IsVisible = blogPost.IsVisible,
            PublishedDate = blogPost.PublishedDate,
            ShortDescription = blogPost.ShortDescription,
            Title = blogPost.Title,
            UrlHandle = blogPost.UrlHandle,
            Categories = blogPost.Categories.Select(p => new CategoryDto
            {
                Id = p.Id,
                Name = p.Name,
                UrlHandle = p.UrlHandle
            }).ToList()
        };
        return Ok(response);
    }

    //PUT: {apiBaseUrl}/api/blogposts/{id}
    [HttpPut]
    [Route("{id:Guid}")]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> UpdateBlogPostById([FromRoute] Guid id, UpdateBlogPostRequestDto request)
    {
        //Convert DTO to Domain Model
        var blogPost = new BlogPost
        {
            Id = id,
            Author = request.Author,
            Content = request.Content,
            CoverImageUrl = request.CoverImageUrl,
            IsVisible = request.IsVisible,
            PublishedDate = request.PublishedDate,
            ShortDescription = request.ShortDescription,
            Title = request.Title,
            UrlHandle = request.UrlHandle,
            Categories = new List<Category>()
        };

        //Foreach
        foreach (var categoryGuid in request.Categories)
        {
           var existingCategory = await categoryRepository.GetById(categoryGuid);
            if (existingCategory is not null)
            {
                blogPost.Categories.Add(existingCategory);
            }
        }

        //Call the repository to update the blog post
        var updatedBlogPost =await blogPostRepository.UpdateAsync(blogPost);

        if (updatedBlogPost is null)
        {
            return NotFound();
        }

        //Before we return the response, we have to convert the domain model back to a DTO

        var response = new BlogPostDto()
        {
            Id = blogPost.Id,
            Author = blogPost.Author,
            Content = blogPost.Content,
            CoverImageUrl = blogPost.CoverImageUrl,
            IsVisible = blogPost.IsVisible,
            PublishedDate = blogPost.PublishedDate,
            ShortDescription = blogPost.ShortDescription,
            Title = blogPost.Title,
            UrlHandle = blogPost.UrlHandle,
            Categories = blogPost.Categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                UrlHandle = c.UrlHandle
            }).ToList()
        };
        return Ok(response);
    }

    //DELETE: {apiBaseUrl}/api/blogposts/{id}
    [HttpDelete]
    [Route("{id:Guid}")]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> DeleteBlogPost([FromRoute] Guid id)
    {
       var deletedBlogpost= await blogPostRepository.DeleteAsync(id);
        if (deletedBlogpost is null)
            {
                return NotFound();
            }
        
       //Convert the domain model to DTO
        var response = new BlogPostDto
        {
            Id = deletedBlogpost.Id,
            Author = deletedBlogpost.Author,
            Content = deletedBlogpost.Content,
            CoverImageUrl = deletedBlogpost.CoverImageUrl,
            IsVisible = deletedBlogpost.IsVisible,
            PublishedDate = deletedBlogpost.PublishedDate,
            ShortDescription = deletedBlogpost.ShortDescription,
            Title = deletedBlogpost.Title,
            UrlHandle = deletedBlogpost.UrlHandle,
        };
       
        return Ok(response);    
    }
}
