﻿using AutoMapper;
using BlogApp.WebApi.Data;
using BlogApp.WebApi.Data.Entities;
using BlogApp.WebApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.WebApi.Controllers;
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
            var existingCategory = await _context.Categories.FindAsync(categoryGuid);
            if (existingCategory is not null)
            {
                blogPost.Categories.Add(existingCategory);
            }
        }

        await _context.BlogPosts.AddAsync(blogPost);
        await _context.SaveChangesAsync();

        //Before we return the response, we have to convert the domain model back to a DTO
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
            Categories = blogPost.Categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                UrlHandle = c.UrlHandle
            }).ToList()
        };

        return Ok(response);
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(QueryBlogPostsResponse))]
    public async Task<IActionResult> QueryBlogPosts()
    {
        var blogPosts = await _context.BlogPosts.Include(p => p.Categories).ToListAsync();

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
            Categories = blogPost.Categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                UrlHandle = c.UrlHandle
            }).ToList()
        };

        return Ok(response);
    }

    [HttpGet("{urlHandle}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetBlogPostResponse))]
    public async Task<IActionResult> GetBlogPost([FromRoute] string urlHandle)
    {
        //Get blogPost details from DB
        var blogPost = await _context.BlogPosts.FirstOrDefaultAsync(p => p.UrlHandle == urlHandle);

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
            Categories = blogPost.Categories.Select(p => new CategoryDto
            {
                Id = p.Id,
                Name = p.Name,
                UrlHandle = p.UrlHandle
            }).ToList()
        };
        return Ok(response);
    }

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

        //Update the domain model
       _mapper.Map(request, blogPost);

        //Foreach
        foreach (var categoryGuid in request.Categories)
        {
            var existingCategory = await _context.Categories.FindAsync(categoryGuid);

            if (existingCategory is not null)
            {
                blogPost.Categories.Add(existingCategory);
            }
        }

        await _context.SaveChangesAsync();

        //Before we return the response, we have to convert the domain model back to a DTO
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
            Categories = blogPost.Categories.Select(c => new CategoryDto
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
