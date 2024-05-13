using BlogApp.WebApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.WebApi.Data;

public class AppDbContext :DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BlogImage> BlogImages { get; set; }
}
