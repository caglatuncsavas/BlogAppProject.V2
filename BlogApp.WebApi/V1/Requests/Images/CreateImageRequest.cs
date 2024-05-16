﻿namespace BlogApp.WebApi.V1.Requests.Images;

public class CreateImageRequest
{
    public IFormFile File { get; set; }
    public string FileName { get; set; }
    public string Title { get; set; }
}
