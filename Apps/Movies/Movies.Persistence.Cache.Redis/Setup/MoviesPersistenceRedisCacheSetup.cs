using Microsoft.Extensions.DependencyInjection;
using Movies.Persistence.Cache.Redis.Repos;
using Movies.Shared.Repos;

namespace Movies.Persistence.Cache.Redis.Setup;

public static class MoviesPersistenceRedisCacheSetup
{
    public static IServiceCollection AddMoviesPersistenceCache_Redis(this IServiceCollection services, Action<MoviesPersistenceRedisCacheSetupOptions> config)
    {
        MoviesPersistenceRedisCacheSetupOptions setupOptions = new();
        config(setupOptions);
        return services.AddMoviesPersistenceCache_Redis(setupOptions);
    }

    //- - - - - - - - - - - - - //

    public static IServiceCollection AddMoviesPersistenceCache_Redis(this IServiceCollection services, MoviesPersistenceRedisCacheSetupOptions options)
    {
        services.AddMemoryCache();
        services.AddStackExchangeRedisCache(redisOptions =>
        {
            redisOptions.Configuration = options.RedisConnection;
        });
        //Scrutor
        services.Decorate<IMoviesRepo, MoviesRepoRedisCached>();
        return services;
    }

}//Cls
