using AutoMapper;

namespace MoviesApi.Helpers
{
    public class MappingProfile:Profile
    {

        public MappingProfile()
        {
            CreateMap<Movie, MovieDetailsDto>().ReverseMap();
            CreateMap<MovieDto, Movie>()
                .ForMember(M => M.Poster, options => options.Ignore());

        }
    }
}
