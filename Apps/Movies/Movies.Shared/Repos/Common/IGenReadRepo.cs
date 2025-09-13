using Movies.Shared.Models;

namespace Movies.Shared.Repos.Common;

/// <summary>
/// Read repo
/// </summary>
public interface IGenReadRepo<T> where T : MovieBaseDomainEntity
{
    /// <summary>
    /// Gets the number of Items in the Db
    /// </summary>
    Task<int> CountAsync();

    //- - - - - - - - - - - - - //

    /// <summary>
    /// Does an Entity with id, <paramref name="id"/> exist?
    /// </summary>
    /// <param name="id">Entity identifier</param>
    /// <returns>True if entity was found</returns>
    Task<bool> ExistsAsync(int? id);

    //- - - - - - - - - - - - - //

    /// <summary>
    /// Get the entity with id, <paramref name="id"/>
    /// </summary>
    /// <param name="id">Entity identifier</param>
    /// <returns>The entity if found, otherwise null</returns>
    Task<T?> FirstOrDefaultByIdAsync(int? id);

    //- - - - - - - - - - - - - //

    /// <summary>
    /// Get all entities
    /// </summary>
    /// <returns>List of all entities</returns>
    Task<IReadOnlyList<T>> ListAllAsync();

    //- - - - - - - - - - - - - //

    /// <summary>
    /// Retrieves a list of entities by their identifiers.
    /// </summary>
    /// <param name="ids">The identifiers of the entities.</param>
    /// <returns>List of entities</returns>
    Task<IReadOnlyList<T>> ListByIdsAsync(IEnumerable<int>? ids);

    //- - - - - - - - - - - - - //

    /// <summary>
    /// Gets first <paramref name="count"/> Entities, after skipping <paramref name="skip"/> entities.
    /// </summary>
    /// <param name="count">The number of entities to retrieve.</param>
    /// <param name="skip">The number of entities to skip.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of <paramref name="count"/> entities</returns>
    Task<IReadOnlyList<T>> TakeAsync(int count, int skip, CancellationToken cancellationToken = default);


}//int
