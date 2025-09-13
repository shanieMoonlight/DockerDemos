using Movies.Shared.Models;

namespace Movies.API;

public class MoviesServiceSimple(ILogger<MoviesServiceSimple> _logger) : IMoviesService
{
    private readonly List<Movie> _movies = CreateMovies();

    private static List<Movie> CreateMovies()
    {
        var list = new List<Movie>
        {
            Movie.Create("Die Another Day", "Lee Tamahori", 2002),
            Movie.Create("Top Gun", "Tony Scott", 1986),
            Movie.Create("Jurassic Park", "Steven Spielberg", 1993),
            Movie.Create("Independence Day", "Roland Emmerich", 1996),
            Movie.Create("Tomorrow Never Dies", "Roger Spottiswoode", 1997)
        };

        for (int i = 0; i < list.Count; i++)
        {
            list[i].Id = i + 1;
        }

        return list;
    }

    //-------------------------//

    public Task<Movie?> GetMovieAsync(int id)
    {
        var movie = _movies.FirstOrDefault(m => m.Id == id);
        return Task.FromResult(movie);
    }

    //-------------------------//

    public Task<IEnumerable<Movie>> GetMoviesAsync()
    {
        _logger.LogInformation("Getting movies from MoviesService");
        return Task.FromResult<IEnumerable<Movie>>(_movies);
    }

    //-------------------------//

    public Task SeedDataAsync()
    {
        _logger.LogInformation("Seeding Movies");
        return Task.CompletedTask;
    }

}//Cls
