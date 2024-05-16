using AutoMapper;
using BlogApp.WebApi.Data;
using BlogApp.WebApi.Data.Entities;
using BlogApp.WebApi.V1.Requests.BlogPosts;
using BlogApp.WebApi.V1.Responses.BlogPosts;
using BlogApp.WebApi.V1.Responses.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.WebApi.V1.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BlogPostsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    public BlogPostsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateBlogPostResponse))]
    public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostRequest request)
    {
        // Convert DTO to Domain Model
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
            var existingCategory = await _context.Categories.FindAsync(categoryGuid);
            if (existingCategory is not null)
            {
                blogPost.Categories.Add(existingCategory);
            }
        }

        await _context.BlogPosts.AddAsync(blogPost);
        await _context.SaveChangesAsync();

        // Before we return the response, we have to convert the domain model back to a DTO
        var response = new CreateBlogPostResponse
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
            Categories = blogPost.Categories.Select(c => new CreateCategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                UrlHandle = c.UrlHandle
            }).ToList()
        };

        return CreatedAtAction(nameof(CreateBlogPost), new { id = blogPost.Id }, response);
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<QueryBlogPostsResponse>))]
    public async Task<IActionResult> QueryBlogPosts()
    {
        var blogPosts = await _context.BlogPosts.Include(p => p.Categories).ToListAsync();


        List<QueryBlogPostsResponse> response = blogPosts.Select(p => new QueryBlogPostsResponse
        {
            Title = p.Title,
            ShortDescription = p.ShortDescription,
            Content = p.Content,
            CoverImageUrl = p.CoverImageUrl,
            UrlHandle = p.UrlHandle,
            Author = p.Author,
            IsVisible = p.IsVisible,
            PublishedDate = p.PublishedDate,
            Categories = p.Categories.Select(p=> new CreateCategoryResponse
            {
                Id = p.Id,
                Name = p.Name,
                UrlHandle = p.UrlHandle
            }).ToList()
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetBlogPostResponse))]
    public async Task<IActionResult> GetBlogPost([FromRoute] Guid id)
    {
        var blogPost = await _context.BlogPosts.Include(p => p.Categories).FirstOrDefaultAsync(p => p.Id == id);

        if (blogPost is null)
        {
            return NotFound();
        }

        GetBlogPostResponse response = new GetBlogPostResponse
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
            Categories = blogPost.Categories.Select(c => new CreateCategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                UrlHandle = c.UrlHandle
            }).ToList()
        };

        return Ok(response);
    }

    //[HttpGet("{urlHandle}")]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetBlogPostResponse))]
    //public async Task<IActionResult> GetBlogPost([FromRoute] string urlHandle)
    //{
    //    //Get blogPost details from DB
    //    var blogPost = await _context.BlogPosts.FirstOrDefaultAsync(p => p.UrlHandle == urlHandle);

    //    if (blogPost is null)
    //    {
    //        return NotFound();
    //    }

    //    GetBlogPostResponse response = new GetBlogPostResponse
    //    {
    //        Id = blogPost.Id,
    //        Author = blogPost.Author,
    //        Content = blogPost.Content,
    //        CoverImageUrl = blogPost.CoverImageUrl,
    //        IsVisible = blogPost.IsVisible,
    //        PublishedDate = blogPost.PublishedDate,
    //        ShortDescription = blogPost.ShortDescription,
    //        Title = blogPost.Title,
    //        UrlHandle = blogPost.UrlHandle,
    //        Categories = blogPost.Categories.Select(p => new CreateCategoryResponse
    //        {
    //            Id = p.Id,
    //            Name = p.Name,
    //            UrlHandle = p.UrlHandle
    //        }).ToList()
    //    };
    //    return Ok(response);
    //}

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateBlogPostResponse))]
    public async Task<IActionResult> UpdateBlogPostById([FromRoute] Guid id, [FromBody] UpdateBlogPostRequest request)
    {
        var blogPost = await _context.BlogPosts.Include(p => p.Categories).FirstOrDefaultAsync(p => p.Id == id);

        if (blogPost is null)
        {
            return NotFound();
        }

        // Update the domain model
        _mapper.Map(request, blogPost);

        // Foreach
        foreach (var categoryGuid in request.Categories)
        {
            var existingCategory = await _context.Categories.FindAsync(categoryGuid);

            if (existingCategory is not null)
            {
                blogPost.Categories.Add(existingCategory);
            }
        }

        await _context.SaveChangesAsync();

        // Before we return the response, we have to convert the domain model back to a DTO
        var response = new UpdateBlogPostResponse()
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
            Categories = blogPost.Categories.Select(c => new CreateCategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                UrlHandle = c.UrlHandle
            }).ToList()
        };
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Writer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteBlogPost([FromRoute] Guid id)
    {
        var blogPost = await _context.BlogPosts.FindAsync(id);

        if (blogPost is null)
        {

            return NoContent();
        }

        _context.BlogPosts.Remove(blogPost);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
