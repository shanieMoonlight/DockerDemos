using Movies.Shared.Models;

namespace Movies.Persistence.Ef.Repos.Abs;
internal abstract class ABaseRepo<T>(MoviesDbContext db) where T : MovieBaseDomainEntity
{
    protected MoviesDbContext Db = db;
 
}//Cls
