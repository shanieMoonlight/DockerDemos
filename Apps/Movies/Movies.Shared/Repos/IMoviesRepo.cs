using Movies.Shared.Models;
using Movies.Shared.Repos.Common;

namespace Movies.Shared.Repos;
public interface IMoviesRepo : IGenCrudRepo<Movie>
{

    ///// <summary>
    ///// Does an Entity with id, <paramref name="id"/> exist?
    ///// </summary>
    ///// <param name="id">Entity identifier</param>
    ///// <returns>True if entity was found</returns>
    //Task<bool> ExistsAsync(int? id);

    ////- - - - - - - - - - - - - //

    ///// <summary>
    ///// Get the entity with id, <paramref name="id"/>
    ///// </summary>
    ///// <param name="id">Entity identifier</param>
    ///// <returns>The entity if found, otherwise null</returns>
    //Task<Movie?> FirstOrDefaultByIdAsync(int? id);

    ////- - - - - - - - - - - - - //

    ///// <summary>
    ///// Get all entities
    ///// </summary>
    ///// <returns>List of all entities</returns>
    //Task<IReadOnlyList<Movie>> ListAllAsync();

    ////- - - - - - - - - - - - - //


    ///// <summary>
    ///// Delete <paramref name="entity"/>
    ///// </summary>
    ///// <param name="entity">Database item</param>
    ///// <returns></returns>
    //Task DeleteAsync(Movie entity);

    ////- - - - - - - - - - - - - //

    ///// <summary>
    ///// Delete entity with id, <paramref name="id"/>
    ///// </summary>
    ///// <param name="id">Entity identifier</param>
    ///// <returns></returns>
    //Task DeleteAsync(int? id);

    ////- - - - - - - - - - - - - //

    ///// <summary>
    ///// Remove a range of entities.
    ///// </summary>
    ///// <param name="entities">Collection of entities to be removed</param>
    ///// <returns></returns>
    //Task RemoveRangeAsync(IEnumerable<Movie> entities);

    ////- - - - - - - - - - - - - //

    ///// <summary>
    ///// Add entity to DB
    ///// </summary>
    ///// <param name="entity">Database item</param>
    ///// <returns>Returns the added entity</returns>
    //Task<Movie> AddAsync(Movie entity, CancellationToken cancellationToken = default);

    ////- - - - - - - - - - - - - //

    ///// <summary>
    ///// Update entity
    ///// </summary>
    ///// <param name="entity">Database item</param>
    ///// <returns>The updated entity</returns>
    //Task<Movie> UpdateAsync(Movie entity);

    ////- - - - - - - - - - - - - //

}//Cls
