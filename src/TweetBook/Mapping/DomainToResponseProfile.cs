using AutoMapper;
using System.Linq;
using TweetBook.Contracts.V1.Responses;
using TweetBook.Domain;

namespace TweetBook.Mapping
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Post, PostResponse>()
                .ForMember(dest => dest.Tags,
                            opt => opt.MapFrom(src => src.Tags.Select(t => new TagResponse { Name = t.TagName }).ToList()));

            CreateMap<Tag, TagResponse>();
        }
    }
}