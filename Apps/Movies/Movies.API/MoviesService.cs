using Movies.Shared.Models;
using Movies.Shared.Repos;

namespace Movies.API;

public class MoviesService(
    IMoviesUnitOfWork _uow,
    IMoviesRepo _repo,
    ILogger<MoviesService> _logger) 
    : IMoviesService
{

    public async Task<IEnumerable<Movie>> GetMoviesAsync()
    {
        _logger.LogInformation("Getting movies from MoviesService");
        return await _repo.ListAllAsync();
    }

    //--------------------//
    public async Task<Movie?> GetMovieAsync(int id)
    {
        _logger.LogInformation("Getting movie with id {MovieId} from MoviesService", id);
        return await _repo.FirstOrDefaultByIdAsync(id);
    }

    //--------------------//

    public async Task SeedDataAsync()
    {
        var currentMovies = await _repo.ListAllAsync();
        if (currentMovies.Any())
            return;

        await _repo.AddRangeAsync(
        [
            Movie.Create("Die Another Day", "Lee Tamahori", 2002),
            Movie.Create("Top Gun", "Tony Scott", 1986),
            Movie.Create("Jurassic Park", "Steven Spielberg", 1993),
            Movie.Create("Independence Day", "Roland Emmerich", 1996),
            Movie.Create("Tomorrow Never Dies", "Roger Spottiswoode", 1997)
        ]);

        await _uow.SaveChangesAsync();
    }



}//Cls
