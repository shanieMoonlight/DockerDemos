using Microsoft.AspNetCore.Mvc;
using Movies.Shared.Models;
using Movies.Shared.Repos.Services;

namespace Movies.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController(
    IMoviesService _moviesService,
    IDbMntcService _dbMntcService
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

    [HttpGet("test2")]
    public ActionResult<IEnumerable<Movie>> Hello() => 
        Ok("Hello");


    //---------------------------//

}
