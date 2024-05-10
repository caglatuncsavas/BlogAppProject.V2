using BlogApp.WebApi.Data.Entities;
using System.Net;

namespace BlogApp.WebApi.Repositories.Interface;

public interface IImageRepository
{
    Task<BlogImage> Upload(IFormFile file, BlogImage blogImage);
    Task<IEnumerable<BlogImage>> GetAll();
}
