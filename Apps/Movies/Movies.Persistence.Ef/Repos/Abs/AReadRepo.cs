using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Movies.Shared.Models;
using Movies.Shared.Repos.Common;

namespace Movies.Persistence.Ef.Repos.Abs;
internal abstract class AReadRepo<T>(MoviesDbContext db)
    : ABaseRepo<T>(db), IGenReadRepo<T> where T : MovieBaseDomainEntity
{


    /// <summary>
    /// Gets the count of all entities.
    /// </summary>
    /// <returns>The count of all entities.</returns>
    public async Task<int> CountAsync() =>
      await Db.Set<T>().CountAsync();

    //--------------------------// 


    /// <summary>
    /// Checks if an entity with the specified ID exists.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <returns>True if the entity exists, otherwise false.</returns>
    public async Task<bool> ExistsAsync(int? id)
    {
        if (id == null)
            return false;

        var entity = await FirstOrDefaultByIdAsync(id);

        return entity is not null;
    }

    //--------------------------// 

    /// <summary>
    /// Retrieves the first entity  in the Database (ordered by Id).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The entity if it exists.</returns>
    public virtual async Task<T?> FirstOrDefaultAsync(CancellationToken cancellationToken = default) =>
        await Db.Set<T>()
            .OrderBy(item => item.Id)
            .FirstOrDefaultAsync(cancellationToken);

    //--------------------------// 

    /// <summary>
    /// Retrieves the first entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <returns>The entity if it exists.</returns>
    public async Task<T?> FirstOrDefaultByIdAsync(int? id) =>
        await Db.Set<T>().FindAsync(id);

    //--------------------------// 


    /// <summary>
    /// Gets a list of all entities.
    /// </summary>
    /// <returns>A list of all entities.</returns>
    public async Task<IReadOnlyList<T>> ListAllAsync() =>
        await Db.Set<T>().ToListAsync();

    //--------------------------// 

    /// <summary>
    /// Gets a list of entities by their IDs.
    /// </summary>
    /// <param name="ids">The IDs of the entities.</param>
    /// <returns>A list of entities.</returns>
    public async Task<IReadOnlyList<T>> ListByIdsAsync(IEnumerable<int>? ids)
    {
        if (ids.IsNullOrEmpty())
            return []; //Don't waste time querying the DB

        return await Db.Set<T>()
            .Where(item => ids!.Contains(item.Id))
            .ToListAsync();
    }

    //--------------------------// 

    /// <summary>
    /// Gets first <paramref name="count"/> Entities, after skipping <paramref name="skip"/> entities.
    /// Whithout any where criteria.
    /// </summary>
    /// <param name="count">The number of entities to retrieve.</param>
    /// <param name="skip">The number of entities to skip.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of <paramref name="count"/> entities.</returns>
    public async Task<IReadOnlyList<T>> TakeAsync(int count, int skip, CancellationToken cancellationToken = default) =>
        await Db.Set<T>()
                .Skip(skip)
                .Take(count)
                .ToListAsync(cancellationToken);

    //--------------------------// 

}//Cls
