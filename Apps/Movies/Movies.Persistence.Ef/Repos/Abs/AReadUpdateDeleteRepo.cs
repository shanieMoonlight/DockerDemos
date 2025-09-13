using Movies.Shared.Models;
using Movies.Shared.Repos.Common;

namespace Movies.Persistence.Ef.Repos.Abs;
internal abstract class AReadUpdateDeleteRepo<T>(MoviesDbContext _db) 
    : AReadUpdateRepo<T>(_db), IGenDeleteRepo<T> where T : MovieBaseDomainEntity
{
    /// <summary>
    /// Delete <paramref name="entity"/>
    /// </summary>
    /// <exception cref="CantDeleteException"></exception>
    public virtual Task DeleteAsync(T entity)
    {
        Db.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    //- - - - - - - - - - - - - //

    /// <summary>
    /// Delete item with id <paramref name="id"/>
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <returns></returns>
    /// <exception cref="CantDeleteException"></exception>
    public virtual async Task DeleteAsync(int? id)
    {
        if (id == null)
            return;

        var entity = await Db.Set<T>()
            .FindAsync(id);

        if (entity == null) //Already Deleted
            return;

        await DeleteAsync(entity);
    }

    //--------------------------// 

    /// <inheritdoc/>
    public Task RemoveRangeAsync(IEnumerable<T> entities)
    {
        Db.Set<T>().RemoveRange(entities);
        return Task.CompletedTask;
    }


}//Cls
