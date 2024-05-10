using BlogApp.WebApi.Data.Entities;
using BlogApp.WebApi.DTOs;
using BlogApp.WebApi.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ImagesController : ControllerBase
{
    private readonly IImageRepository imageRepository;

    public ImagesController(IImageRepository imageRepository)
    {
        this.imageRepository = imageRepository;
    }
    // POST: {apibaseurl}/api/Images
    [HttpPost]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromForm] string fileName, [FromForm] string title)
    {
        ValidateFileUpload(file);

        if (ModelState.IsValid)
        {
            //File Upload
            var blogImage = new BlogImage
            {
                FileExtension = Path.GetExtension(file.FileName).ToLower(),
                FileName = fileName,
                Title = title,
                DateCreated = DateTime.Now
            };

            blogImage = await imageRepository.Upload(file, blogImage);

            //Convert Domain Model to DTO

            var response = new BlogImageDto
            {
                Id = blogImage.Id,
                FileName = blogImage.FileName,
                FileExtension = blogImage.FileExtension,
                Title = blogImage.Title,
                Url = blogImage.Url,
                DateCreated = blogImage.DateCreated
            };

            return Ok(response);
        }

        return BadRequest(ModelState);
    }

    private void ValidateFileUpload(IFormFile file)
    {
        var allowExtensions = new string[] {".jpg", ".jpeg", ".png" };

        if (!allowExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
        {
            ModelState.AddModelError("file", "Unsupported file format.");
        }

        if(file.Length > 10485760)
        {
            ModelState.AddModelError("file", "File size cannot be more than 10Mb.");
        }
    }

    // GET: {apibaseurl}/api/Images
    [HttpGet]
    public async Task<IActionResult> GetAllImages()
    {
        // Call image repository to get all images
        var images = await imageRepository.GetAll();

        // Convert Domain Model to DTO
        var response = new List<BlogImageDto>();
       foreach(var image in images)
        {
            response.Add(new BlogImageDto
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
