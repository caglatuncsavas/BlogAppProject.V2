using BlogApp.WebApi.Context;
using BlogApp.WebApi.Data.Entities;
using BlogApp.WebApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.WebApi.Repositories.Implementation;

public class ImageRepository : IImageRepository
{
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly AppDbContext context;

    public ImageRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, AppDbContext context)
    {
        this.webHostEnvironment = webHostEnvironment;
        this.httpContextAccessor = httpContextAccessor;
        this.context = context;
    }
    public async Task<BlogImage> Upload(IFormFile file, BlogImage blogImage)
    {
        //1.Upload the Image to API/Images
        var localPath = Path.Combine(webHostEnvironment.ContentRootPath, "Images", $"{blogImage.FileName}{blogImage.FileExtension}");

        using var stream = new FileStream(localPath, FileMode.Create);
        await file.CopyToAsync(stream);

        //2. Save the Image to the Database - Update the database
        //https://blogApp.com/images/somefilenamejpg

        var httpRequest = httpContextAccessor.HttpContext.Request;
        var urlPath = $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.PathBase}/Images/{blogImage.FileName}{blogImage.FileExtension}";


        blogImage.Url = urlPath;
        
        await context.BlogImages.AddAsync(blogImage);    
        await context.SaveChangesAsync();

        return blogImage;

    }
    public async Task<IEnumerable<BlogImage>> GetAll()
    {
       return await context.BlogImages.ToListAsync();
    }

}
