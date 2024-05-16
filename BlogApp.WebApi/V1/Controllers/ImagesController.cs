using BlogApp.WebApi.Data;
using BlogApp.WebApi.Data.Entities;
using BlogApp.WebApi.V1.Requests.Images;
using BlogApp.WebApi.V1.Responses.Images;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.WebApi.V1.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ImagesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public ImagesController(AppDbContext context, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateImageResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> CreateImage([FromForm] CreateImageRequest request)
    {
        ValidateFileUpload(request.File);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var fileExtension = Path.GetExtension(request.File.FileName).ToLower();

        var blogImage = new BlogImage
        {
            FileExtension = fileExtension,
            FileName = request.FileName,
            Title = request.Title,
            DateCreated = DateTime.Now
        };
        blogImage = await Upload(request.File, blogImage);

        // Convert Domain Model to DTO
        var response = new CreateImageResponse
        {
            Id = blogImage.Id,
            FileName = blogImage.FileName,
            FileExtension = blogImage.FileExtension,
            Title = blogImage.Title,
            Url = blogImage.Url,
            DateCreated = blogImage.DateCreated
        };
        return CreatedAtAction(nameof(CreateImage), new { id = blogImage.Id }, response);
    }
    private async Task<BlogImage> Upload(IFormFile file, BlogImage blogImage)
    {
        var localPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", $"{blogImage.FileName}{blogImage.FileExtension}");

        using (var stream = new FileStream(localPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var httpRequest = _httpContextAccessor.HttpContext!.Request;
        var urlPath = $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.PathBase}/Images/{blogImage.FileName}{blogImage.FileExtension}";

        blogImage.Url = urlPath;

        _context.BlogImages.Add(blogImage);
        await _context.SaveChangesAsync();

        return blogImage;
    }
    private void ValidateFileUpload(IFormFile file)
    {
        var allowExtensions = new string[] { ".jpg", ".jpeg", ".png" };

        if (!allowExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
        {
            ModelState.AddModelError("file", "Unsupported file format.");
        }

        if (file.Length > 10485760)
        {
            ModelState.AddModelError("file", "File size cannot be more than 10Mb.");
        }
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<QueryBlogImageResponse>))]
    public async Task<IActionResult> QueryImages()
    {
        var images = await _context.BlogImages.ToListAsync();

        // Convert Domain Model to DTO
        var response = new List<QueryBlogImageResponse>();

        foreach (var image in images)
        {
            response.Add(new QueryBlogImageResponse
            {
                Id = image.Id,
                FileName = image.FileName,
                FileExtension = image.FileExtension,
                Title = image.Title,
                Url = image.Url,
                DateCreated = image.DateCreated
            });
        }
        return Ok(response);
    }
}
