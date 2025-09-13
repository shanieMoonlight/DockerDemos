using Microsoft.Extensions.DependencyInjection;
using Movies.Persistence.Cache.Memory.Repos;
using Movies.Shared.Repos;

namespace Movies.Persistence.Cache.Memory.Setup;

public static class MoviesPersistenceMemCacheSetup
{
    public static IServiceCollection AddMoviesPersistenceCache_Memory(this IServiceCollection services, Action<MoviesPersistenceMemCacheSetupOptions> config)
    {
        MoviesPersistenceMemCacheSetupOptions setupOptions = new();
        config(setupOptions);
        return services.AddMoviesPersistenceCache_Memory(setupOptions);
    }

    //- - - - - - - - - - - - - //

    public static IServiceCollection AddMoviesPersistenceCache_Memory(this IServiceCollection services, MoviesPersistenceMemCacheSetupOptions options)
    {
        services.AddMemoryCache();
        //Scrutor
        services.Decorate<IMoviesRepo, MoviesRepoMemoryCached>();
        return services;
    }

}//Cls
