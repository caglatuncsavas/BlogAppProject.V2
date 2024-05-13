using BlogApp.WebApi.Data;
using BlogApp.WebApi.Data.Entities;
using BlogApp.WebApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.WebApi.Controllers;

// https://localhost:xxxx/api/categories
[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        context = _context;
    }

    [HttpPost]
    //[Authorize(Roles = "Writer")]
    public async Task<IActionResult> CreateCategory([FromBody]CreateCategoryRequestDto request)
    {
       //Map DTO to Domain Model
       var category = new Category
       {
           Name = request.Name,
           UrlHandle = request.UrlHandle
       };

        await _context.Categories.AddAsync(category);

        //Domain model to DTO
        var response = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            UrlHandle = category.UrlHandle
        };


        return Ok(response);
    }

    //GET: https://localhost:7228/api/Categories?query=html&sortBy=name&sortDirection=desc
    [HttpGet]
    public async Task<IActionResult> GetAllCategories([FromQuery] string? query, [FromQuery] string? sortBy, [FromQuery] string? sortDirection,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize)
    {
       //var categories =  await categoryRepository
       //     .GetAllAsync(query, sortBy, sortDirection, pageNumber, pageSize);//Give us all the categories from the database

        var categories = await _context.Categories.ToListAsync();

        //Map Domain Model to DTO

        var response = new List<CategoryDto>();
        foreach (var category in categories)
        {
            response.Add(new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle
            });
        }

        return Ok(response);
    }

    //GET: https://localhost:7228/api/Categories/{id}
    [HttpGet]
    [Route("{id:Guid}")]
    public async Task<IActionResult> GetCategoriesById([FromRoute]Guid id)
    {
        var existingCategory = await _context.Categories.FindAsync(id);

        if(existingCategory is null)
        {
            return NotFound();
        }

        var response = new CategoryDto
        {
            Id = existingCategory.Id,
            Name = existingCategory.Name,
            UrlHandle = existingCategory.UrlHandle
        };

        return Ok(response);

    }

    //PUT: https://localhost:7228/api/Categories/{id}
    [HttpPut]
    [Route("{id:Guid}")]
    //[Authorize(Roles = "Writer")]
    public async Task<IActionResult> EditCategory([FromRoute] Guid id, UpdateCategoryRequestDto request)
    {
        //Convert DTO to Domain Model
        var category = new Category
        {
            Id = id,
            Name = request.Name,
            UrlHandle = request.UrlHandle
        };

        //Update the category in the database  ????????????????


        category = await _context.Categories.FindAsync(id);

        if(category is null)
        {
            return NotFound();
        }

        //Convert Domain Model to DTO
        var response = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            UrlHandle = category.UrlHandle
        };

        return Ok(response);
    }

    //DELETE: https://localhost:7228/api/Categories/{id}

    [HttpDelete]
    [Route("{id:Guid}")]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
    {
       var category = await _context.Categories.FindAsync(id);
        if (category is null)
        {
            return NotFound();
        }

        //Convert Domain Model to DTO

        var response = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            UrlHandle = category.UrlHandle
        };
        return Ok(response);

    }

    
    //GET: https://localhost:7228/api/Categories/count
    [HttpGet]
    [Route("count")]
    //[Authorize(Roles = "Writer")]
    public async Task<IActionResult> GetCategoriesTotal()
    {
        var count = await _context.Categories.CountAsync();

        return Ok(count);
    }
}

