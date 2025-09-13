using Movies.Shared.Models;

namespace Movies.Shared.Repos.Common;

/// <summary>
///  Update
/// </summary>
public interface IGenUpdateRepo<T> where T : MovieBaseDomainEntity
{

    /// <summary>
    /// Update entity
    /// </summary>
    /// <param name="entity">Database item</param>
    /// <returns>The updated entity</returns>
    Task<T> UpdateAsync(T entity);

    //- - - - - - - - - - - - - //

    /// <summary>
    /// Starts tracking entities for updates
    /// </summary>
    /// <param name="entity">Database item</param>
    Task UpdateRangeAsync(IEnumerable<T> entities);

}//int
