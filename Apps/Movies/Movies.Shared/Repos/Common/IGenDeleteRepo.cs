using Movies.Shared.Models;

namespace Movies.Shared.Repos.Common;

/// <summary>
///  Delete repo
/// </summary>
public interface IGenDeleteRepo<T> where T : MovieBaseDomainEntity
{

    /// <summary>
    /// Delete <paramref name="entity"/>
    /// </summary>
    /// <param name="entity">Database item</param>
    /// <returns></returns>
    Task DeleteAsync(T entity);

    //- - - - - - - - - - - - - //

    /// <summary>
    /// Delete entity with id, <paramref name="id"/>
    /// </summary>
    /// <param name="id">Entity identifier</param>
    /// <returns></returns>
    Task DeleteAsync(int? id);

    //- - - - - - - - - - - - - //

    /// <summary>
    /// Remove a range of entities.
    /// </summary>
    /// <param name="entities">Collection of entities to be removed</param>
    /// <returns></returns>
    Task RemoveRangeAsync(IEnumerable<T> entities);


}//int
