using Movies.Shared.Models;

namespace Movies.Shared.Repos.Common;

/// <summary>
/// Create, Read, Update, Delete repo
/// </summary>
public interface IGenCrudRepo<T> :
    IGenCreateRepo<T>,
    IGenReadRepo<T>,
    IGenUpdateRepo<T>,
    IGenDeleteRepo<T> where T : MovieBaseDomainEntity
{ }
