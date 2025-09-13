using Movies.Shared.Models;

namespace Movies.Shared.Repos.Common;

/// <summary>
/// Create
/// </summary>
public interface IGenCreateRepo<T> where T : MovieBaseDomainEntity
{

    /// <summary>
    /// Add entity to DB
    /// </summary>
    /// <param name="entity">Database item</param>
    /// <returns>Returns the added entity</returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    //- - - - - - - - - - - - - //
    /// <summary>
    /// Enter Multiple Items at one
    /// </summary>
    /// <param name="entities">Database items</param>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

}//int
