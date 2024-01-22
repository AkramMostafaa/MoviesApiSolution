
namespace MoviesApi.Services
{
    public class MoviesServices : IMoviesServices
    {
        private readonly ApplicationDbContext _dbContext;

        public MoviesServices(ApplicationDbContext dbContext )
        {
            _dbContext = dbContext;
        }
        public async Task<Movie> Add(Movie movie)
        {
            await _dbContext.AddAsync(movie);
            _dbContext.SaveChanges();
            return movie;
        }

        public Movie Delete(Movie movie)
        {
            _dbContext.Remove(movie);
            _dbContext.SaveChanges();
            return movie;

        }

        public async Task<Movie> GetById(int id)
        => await  _dbContext.Movies.Include(m => m.Genre).SingleOrDefaultAsync(m => m.Id.Equals(id));
        

        public async Task<IEnumerable<Movie>> GetAll(byte genreId = 0)
        {
           return await _dbContext.Movies
                .Where(m=>m.GenreId==genreId || genreId==0)
               .OrderByDescending(m => m.Rate)
               .Include(M => M.Genre)
               .ToListAsync();
        }

        public Movie Update(Movie movie)
        {
            _dbContext.Update(movie);
            _dbContext.SaveChanges();
            return movie;

        }
    }
}
