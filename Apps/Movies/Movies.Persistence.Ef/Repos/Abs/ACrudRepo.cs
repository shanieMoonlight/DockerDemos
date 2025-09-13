using Movies.Shared.Models;
using Movies.Shared.Repos.Common;

namespace Movies.Persistence.Ef.Repos.Abs;
internal abstract class ACrudRepo<T>(MoviesDbContext db)
    : AReadUpdateDeleteRepo<T>(db), IGenCreateRepo<T> where T : MovieBaseDomainEntity
{
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default) =>
        (await Db.Set<T>().AddAsync(entity, cancellationToken)).Entity;

    //--------------------------// 

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default) =>
        await Db.Set<T>().AddRangeAsync(entities, cancellationToken);


}//Cls
