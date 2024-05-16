using AutoMapper;
using BlogApp.WebApi.Data;
using BlogApp.WebApi.Data.Entities;
using BlogApp.WebApi.V1.Requests.Categories;
using BlogApp.WebApi.V1.Responses.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.WebApi.V1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    public CategoriesController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateCategoryResponse))]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var category = new Category
        {
            Name = request.Name,
            UrlHandle = request.UrlHandle
        };

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        // Domain model to DTO
        var response = new CreateCategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            UrlHandle = category.UrlHandle
        };
        return Ok(response);
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<QueryCategoriesResponse>))]
    public async Task<IActionResult> QueryCategories([FromQuery] QueryCategoriesRequest request)
    {
        var queryable = _context.Categories.AsQueryable();

        // Filtering 
        if (!string.IsNullOrEmpty(request.Query))
        {
            queryable = queryable.Where(c => c.Name.Contains(request.Query));
        }

        // Sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            if (request.SortDirection == "asc")
            {
                queryable = queryable.OrderBy(c => c.Name);
            }
            else
            {
                queryable = queryable.OrderByDescending(c => c.Name);
            }
        }

        // Pagination
        if (request.PageNumber.HasValue && request.PageSize.HasValue)
        {
            queryable = queryable.Skip(request.PageNumber.Value - 1 - request.PageSize.Value).Take(request.PageSize.Value);
        }

        var categories = await queryable.ToListAsync();

        // Map Domain Model to DTO
        var response = categories.Select(category => new QueryCategoriesResponse
        {
            Id = category.Id,
            Name = category.Name,
            UrlHandle = category.UrlHandle
        }).ToList();
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCategoryResponse))]
    public async Task<IActionResult> GetCategory([FromRoute] Guid id)
    {
        var existingCategory = await _context.Categories.FindAsync(id);

        if (existingCategory is null)
        {
            return NotFound();
        }

        var response = new CreateCategoryResponse
        {
            Id = existingCategory.Id,
            Name = existingCategory.Name,
            UrlHandle = existingCategory.UrlHandle
        };

        return Ok(response);
    }

    [HttpGet("count")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCategoryCount()
    {
        var count = await _context.Categories.CountAsync();

        return Ok(count);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateCategoryResponse))]
    public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, [FromBody] UpdateCategoryRequest request)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        _mapper.Map(request, category);

        _context.Categories.Update(category);
        await _context.SaveChangesAsync();

        // Convert Domain Model to DTO
        var response = new UpdateCategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            UrlHandle = category.UrlHandle
        };
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Writer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

