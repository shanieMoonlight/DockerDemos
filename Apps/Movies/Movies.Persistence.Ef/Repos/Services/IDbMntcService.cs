using Microsoft.EntityFrameworkCore;
using Movies.Shared.Repos.Services;

namespace Movies.Persistence.Ef.Repos.Services;
public class DbMntcService(MoviesDbContext _db) : IDbMntcService
{
    public async Task MigrateAsync() =>
        await _db.Database.MigrateAsync();

}//Cls
