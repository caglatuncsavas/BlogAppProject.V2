using BlogApp.WebApi.Data.Entities;

namespace BlogApp.WebApi.Repositories.Interface;

public interface ICategoryRepository
{
    Task<Category> CreateAsync(Category category);
    Task<IEnumerable<Category>> GetAllAsync(
        string? query = null, 
        string? sortBy=null, 
        string? sortDirection = null,
        int? pageNumber =1,
        int? pageSized=100 );//This is definiton of getAllAsync method
    Task<Category?> GetById(Guid id);
    Task<Category?> UpdateAsync(Category category);
    Task<Category?> DeleteAsync(Guid id);

    Task<int> GetCount();
}
