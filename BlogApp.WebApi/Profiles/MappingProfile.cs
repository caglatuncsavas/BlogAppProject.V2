using AutoMapper;
using BlogApp.WebApi.Data.Entities;
using BlogApp.WebApi.DTOs;

namespace BlogApp.WebApi.Profiles;

public class MappingProfile : Profile
{
    // Mapping for UpdateBlogPostRequest to BlogPost entity
    public MappingProfile()
    {
        CreateMap<UpdateBlogPostRequest, BlogPost>()
             .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src => src.CoverImageUrl))
            .ForMember(dest => dest.IsVisible, opt => opt.MapFrom(src => src.IsVisible))
            .ForMember(dest => dest.PublishedDate, opt => opt.MapFrom(src => src.PublishedDate))
            .ForMember(dest => dest.ShortDescription, opt => opt.MapFrom(src => src.ShortDescription))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.UrlHandle, opt => opt.MapFrom(src => src.UrlHandle))
            .ForMember(dest => dest.Categories, opt => opt.Ignore());
    }
}
