using BlogApp.WebApi.Context;
using BlogApp.WebApi.Data.Entities;
using BlogApp.WebApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.WebApi.Repositories.Implementation;

public class BlogPostRepository : IBlogPostRepository
{
    private readonly AppDbContext _context;
    public BlogPostRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<BlogPost> CreateAsync(BlogPost blogPost)
    {
        await _context.BlogPosts.AddAsync(blogPost);
        await _context.SaveChangesAsync();
        return blogPost;
    }
    public async Task<IEnumerable<BlogPost>> GetAllAsync()
    {
       return await _context.BlogPosts.Include(p=>p.Categories).ToListAsync();
    }

    public async Task<BlogPost?> GetByIdAsync(Guid id)
    {
        return await _context.BlogPosts.Include(p=> p.Categories).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
    {
        var existingBlogPost = await _context.BlogPosts
            .Include(p=> p.Categories)
            .FirstOrDefaultAsync(p => p.Id == blogPost.Id);

        if (existingBlogPost is null)
        {
            return null;
        }

        //Update BlogPost

        _context.Entry(existingBlogPost).CurrentValues.SetValues(blogPost);

        //Update Categories
        existingBlogPost.Categories = blogPost.Categories;

        await _context.SaveChangesAsync();
        return blogPost;
    }

    public async Task<BlogPost?> DeleteAsync(Guid id)
    {
        var existingBlogPost= await _context.BlogPosts.FirstOrDefaultAsync(p => p.Id == id);
        if (existingBlogPost is not null)
        {
            _context.BlogPosts.Remove(existingBlogPost);
            await _context.SaveChangesAsync();
            return existingBlogPost;
        }
        return null;
    }

    public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
    {
        return await _context.BlogPosts.Include(p => p.Categories).FirstOrDefaultAsync(p => p.UrlHandle == urlHandle);
    }
}
