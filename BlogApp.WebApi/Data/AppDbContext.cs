using BlogApp.WebApi.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.WebApi.Data;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BlogImage> BlogImages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Seed Roles, Seed Default User and Seed role of default user.

        //Cretae Reader and Writer Role

        var readerRoleId = "70e9a0a8-3959-46aa-adf1-8287dc657956";
        var writerRoleId = "3a02e293-22bf-4ec7-b131-d5f8e726fe6f";

        var roles = new List<IdentityRole>
        {
            new IdentityRole{ Id = readerRoleId, Name = "Reader", NormalizedName = "Reader".ToUpper(), ConcurrencyStamp = readerRoleId },
            new IdentityRole{ Id = writerRoleId, Name = "Writer", NormalizedName = "Writer".ToUpper(), ConcurrencyStamp = writerRoleId }
        };

        //Seed the roles

        builder.Entity<IdentityRole>().HasData(roles);

        //Create Default - Admin User
        var adminUserId = "953cc48e-d6ad-4a94-b3ea-b706993d9088";
        var admin = new IdentityUser()
        {
            Id = adminUserId,
            UserName = "admin",
            Email = "admin@gmail.com",
            NormalizedEmail = "admin@gmail.com".ToUpper(),
            NormalizedUserName = "admin".ToUpper()
        };

        admin.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(admin, "Admin@123");

        builder.Entity<IdentityUser>().HasData(admin);

        //Give Roles To Default User - Admin User
        var adminRoles = new List<IdentityUserRole<string>>()
        {
             new()
            {
                UserId = adminUserId,
                RoleId = readerRoleId
            },
            new()
            {
                UserId = adminUserId,
                RoleId = writerRoleId
            }
        };

        builder.Entity<IdentityUserRole<string>>().HasData(adminRoles);
    }
}
