
namespace MoviesApi.Services
{
    public class GenresServices : IGenresService
    {
        private readonly ApplicationDbContext _dbContext;

        public GenresServices(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Genre> Add(Genre genre)
        {
          await  _dbContext.AddAsync(genre);
            _dbContext.SaveChanges();   
            return genre;
        }

        public  Genre Delete(Genre genre)
        {
           _dbContext.Genres.Remove(genre);
            _dbContext.SaveChanges();
            return genre;
        }

        public async Task<IEnumerable<Genre>> GetAll()
        {
            var genre = await _dbContext.Genres.OrderBy(G=>G.Name).ToListAsync();
            return genre;
        }

        public async Task<Genre> GetById(byte id)
        {
            return await _dbContext.Genres.SingleOrDefaultAsync(G => G.Id == id);
        }

        public Task<bool> IsValidGenre(byte id)
        {
           return _dbContext.Genres.AnyAsync(G => G.Id == id);
        }

        public Genre Update(Genre genre)
        {
            _dbContext.Genres.Update(genre);
            _dbContext.SaveChanges();
            return genre;
        }
    }
}
