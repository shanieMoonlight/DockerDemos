using Movies.Shared.Models;

namespace Movies.API;
public interface IMoviesService
{
    Task<Movie?> GetMovieAsync(int id);
    Task<IEnumerable<Movie>> GetMoviesAsync();
    Task<Type> GetRepoTypeAsync();
    Task SeedDataAsync();
}