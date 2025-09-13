using Microsoft.EntityFrameworkCore;
using Movies.Shared.Models;
using Movies.Shared.Repos.Common;

namespace Movies.Persistence.Ef.Repos.Abs;
internal abstract class AReadUpdateRepo<T>(MoviesDbContext db)
    : AReadRepo<T>(db), IGenUpdateRepo<T> where T : MovieBaseDomainEntity
{
    public Task<T> UpdateAsync(T entity)
    {
        Db.Entry(entity).State = EntityState.Modified;
        return Task.FromResult(entity);
    }

    //- - - - - - - - - - - - - - - //

    public Task UpdateRangeAsync(IEnumerable<T> entities)
    {
        Db.Set<T>().UpdateRange(entities);
        return Task.CompletedTask;
    }
}//Cls
