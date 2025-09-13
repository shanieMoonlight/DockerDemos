using Movies.Persistence.Ef.Repos.Abs;
using Movies.Shared.Models;
using Movies.Shared.Repos;

namespace Movies.Persistence.Ef.Repos;
internal class MoviesRepo(MoviesDbContext db) 
    : ACrudRepo<Movie>(db), IMoviesRepo
{


}//Cls
