using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Models;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesServices _moviesServices;
        private readonly IGenresService _genresService;
        private readonly IMapper _mapper;
        private new List<string> _allowedExtentions = new List<string>() { ".jpg",".png"};
        private long _maxAllowedPosterSize = 1048576;
        public MoviesController(IMoviesServices moviesServices,IGenresService genresService,IMapper mapper)
        {
           _moviesServices = moviesServices;
            _genresService = genresService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
         var  movies= await _moviesServices.GetAll();

            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);
            return Ok(data);
        }
        [HttpGet( "{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await _moviesServices.GetById(id);
            if (movie == null)
                return NotFound();

            var dto = _mapper.Map<MovieDetailsDto>(movie);

            return Ok(dto);
        }

        [HttpGet("GetByGenreId")]
        public async Task<IActionResult> GetByGenreIdAsync(byte genreId)
        {
            var movies = await _moviesServices.GetAll(genreId);
            var dto = _mapper.Map<MovieDetailsDto>(movies);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm]MovieDto movieDto)
        {
            if (movieDto.Poster == null)
                return BadRequest("Poster Is Required!");
            if (!_allowedExtentions.Contains(Path.GetExtension(movieDto.Poster.FileName).ToLower()))
                return BadRequest("Only .png and .jpg are allowed!");

            if (movieDto.Poster.Length > _maxAllowedPosterSize)
                return BadRequest("Max allowed size for poster is 1MB!");

            var isValidGenre = await _genresService.IsValidGenre(movieDto.GenreId);

            if (!isValidGenre )
                return BadRequest("Invalid genre ID!");
            using var dataStream = new MemoryStream();
            await movieDto.Poster.CopyToAsync(dataStream);
            var movie = _mapper.Map<Movie>(movieDto );
            movie.Poster=dataStream.ToArray();
           await _moviesServices.Add(movie);
           
            return Ok(movie);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id,[FromForm]MovieDto movieDto)
        {
            var movie = await _moviesServices.GetById(id);
            if (movie is null)
                return NotFound($"No Movie Was Found With ID: {id}");

            var isValidGenre = await _genresService.IsValidGenre(movieDto.GenreId);

            if (!isValidGenre)
                return BadRequest("Invalid genre ID!");
            if (movieDto.Poster != null)
            {
                if (!_allowedExtentions.Contains(Path.GetExtension(movieDto.Poster.FileName).ToLower()))
                    return BadRequest("Only .png and .jpg are allowed!");

                if (movieDto.Poster.Length > _maxAllowedPosterSize)
                    return BadRequest("Max allowed size for poster is 1MB!");

                using var dataStream= new MemoryStream();
                await movieDto.Poster.CopyToAsync(dataStream);
                movie.Poster = dataStream.ToArray();
            }


            movie.Title=movieDto.Title;
            movie.Year=movieDto.Year;
            movie.GenreId=movieDto.GenreId;
            movie.Storyline=movieDto.Storyline;
            movie.Rate=movieDto.Rate;

            _moviesServices.Update(movie);
            return Ok(movie);

           

        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _moviesServices.GetById(id);
            if (movie == null)
                return NotFound($"No Movie Was Found With ID:{id}");

            _moviesServices.Delete(movie);
            return Ok(movie);
                
        }
    }
}
