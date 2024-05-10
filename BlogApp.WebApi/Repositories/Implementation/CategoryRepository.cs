using BlogApp.WebApi.Context;
using BlogApp.WebApi.Data.Entities;
using BlogApp.WebApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.WebApi.Repositories.Implementation;

public class CategoryRepository: ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

 
    public async Task<Category>CreateAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        return category;
    }

    public async Task<Category?> DeleteAsync(Guid id)
    {
        var existingCategory = await _context.Categories.FirstOrDefaultAsync(p => p.Id == id);
        if(existingCategory is null)
        {
            return null;
        }

        _context.Categories.Remove(existingCategory);
        await _context.SaveChangesAsync();
        return existingCategory;
    }

    public async Task<IEnumerable<Category>> GetAllAsync(
        string? query = null, 
        string? sortBy= null, 
        string? sortDirection = null,
        int? pageNumber = 1,
        int? pageSize = 100)
    {
        //Query

        var categories = _context.Categories.AsQueryable();

        //Filtering

        if(string.IsNullOrWhiteSpace(query) is false)
        {
            categories = categories.Where(p => p.Name.Contains(query));
        }

        //Sorting

        if(string.IsNullOrWhiteSpace(sortBy) is false)
        {
            if(string.Equals(sortBy, "Name", StringComparison.OrdinalIgnoreCase))
            {
                var isAsc = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase) ? true : false;

                categories = isAsc ? categories.OrderBy(p => p.Name) : categories.OrderByDescending(p => p.Name);
            }

            if (string.Equals(sortBy, "URL", StringComparison.OrdinalIgnoreCase))
            {
                var isAsc = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase) ? true : false;

                categories = isAsc ? categories.OrderBy(p => p.UrlHandle) : categories.OrderByDescending(p => p.UrlHandle);
            }
        }

        //Pagination
        //Pagenumber 1 pageSize 5 - skip 0, take5
        //Pagenumber 2 pageSize 5 - skip 5, take5
        //Pagenumber 3 pageSize 5 - skip 10, take5

        var skipResults = (pageNumber-1) * pageSize;  
        categories = categories.Skip(skipResults ?? 0).Take(pageSize ?? 100);


        return await categories.ToListAsync();
    }
    public async Task<Category?> GetById(Guid id)
    {
        return await _context.Categories.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<int> GetCount()
    {
       return await _context.Categories.CountAsync();
    }

    public async Task<Category?> UpdateAsync(Category category)
    {
       var existingCategory = await _context.Categories.FirstOrDefaultAsync(p => p.Id == category.Id);

        if (existingCategory is not null)
        {
           _context.Entry(existingCategory).CurrentValues.SetValues(category);
            await _context.SaveChangesAsync();
            return category;
        }
        return null;
    }   
}
