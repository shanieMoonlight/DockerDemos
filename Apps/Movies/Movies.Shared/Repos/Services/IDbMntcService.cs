using Microsoft.Extensions.Hosting;

namespace Movies.Shared.Repos.Services;
public interface IDbMntcService
{
    Task MigrateAsync();
}
