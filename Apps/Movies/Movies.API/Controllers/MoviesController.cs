using Microsoft.AspNetCore.Mvc;
using Movies.Shared.Models;
using Movies.Shared.Repos.Services;

namespace Movies.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController(
    IMoviesService _moviesService,
    IDbMntcService _dbMntcService,
    IConfiguration _configuration
) 
    : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Movie>>> GetMoviesAsync() =>
        Ok(await _moviesService.GetMoviesAsync());


    //---------------------------//

    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<Movie>>> GetMoviesAsync(int id) =>
        Ok(await _moviesService.GetMovieAsync(id));


    //---------------------------//


    [HttpPost("seed")]
    public async Task<IActionResult> SeedDataAsync()
    {
        await _dbMntcService.MigrateAsync();
        await _moviesService.SeedDataAsync();
        return Ok(new { Message= "Migrated and Seeded!!" });
    }

    //---------------------------//

    [HttpGet("test")]
    public async Task<ActionResult<IEnumerable<Movie>>> TestSeAsync()
    {
        var movies = await _moviesService.GetMoviesAsync();
        return Ok(movies);
    }


    //---------------------------//

    [HttpGet("repo")]
    public async Task<ActionResult<Type>> RepoTypeAsync()
    {
        var type = await _moviesService.GetRepoTypeAsync();
        return Ok(type.FullName);
    }


    //---------------------------//

    [HttpGet("test2")]
    public ActionResult<IEnumerable<Movie>> Hello() => 
        Ok("Hello");


    //---------------------------//

    // Debug endpoint: returns the merged AllowedOrigins array from IConfiguration
    [HttpGet("allowed-origins")]
    public ActionResult<string[]> GetAllowedOrigins()
    {
        var origins = _configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
        return Ok(origins);
    }


}
