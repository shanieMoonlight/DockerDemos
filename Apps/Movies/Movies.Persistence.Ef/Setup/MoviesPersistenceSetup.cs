using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Movies.Persistence.Ef.Repos;
using Movies.Persistence.Ef.Repos.Services;
using Movies.Shared.Repos;
using Movies.Shared.Repos.Services;

namespace Movies.Persistence.Ef.Setup;

public static class MoviesPersistenceSetup
{
    public static IServiceCollection AddMoviesPersistenceEf(this IServiceCollection services, Action<MoviesPersistenceSetupOptions> config)
    {
        MoviesPersistenceSetupOptions setupOptions = new();
        config(setupOptions);
        return services.AddMoviesPersistenceEf(setupOptions);
    }

    //- - - - - - - - - - - - - //

    public static IServiceCollection AddMoviesPersistenceEf(this IServiceCollection services, MoviesPersistenceSetupOptions options)
    {
        //services.AddDbContext<MoviesDbContext>(dbOptions =>
        //{
        //    static void providerOptionsAction(SqlServerDbContextOptionsBuilder providerOptions)
        //    {
        //        providerOptions.EnableRetryOnFailure(3);
        //        providerOptions.MigrationsHistoryTable("__EFMigrationsHistory", "Mvs");
        //    }

        //    dbOptions.UseSqlServer(options.ConnectionString, providerOptionsAction);
        //});

        // Use the overload that provides IServiceProvider so you can access IHostEnvironment
        services.AddDbContext<MoviesDbContext>((sp, dbOptions) =>
        {
            var env = sp.GetRequiredService<IHostEnvironment>();

            dbOptions.UseNpgsql(options.ConnectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(3);
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "mvs");
            });

            dbOptions.UseSnakeCaseNamingConvention();

            // Example: enable extra diagnostics only in Development
            if (env.IsDevelopment())
            {
                dbOptions.EnableSensitiveDataLogging();
                dbOptions.LogTo(Console.WriteLine);

            }
        });

        //If IsDevelopment Migrate:

        services.TryAddScoped<IDbMntcService, DbMntcService>();
        services.SetupRepos();

        return services;
    }

    //- - - - - - - - - - - - - //

    private static IServiceCollection SetupRepos(this IServiceCollection services)
    {
        services.TryAddScoped<IMoviesRepo, MoviesRepo>();
        services.TryAddScoped<IMoviesUnitOfWork, MoviesUnitOfWork>();

        return services;
    }

    //--------------------------//

    public static async Task<IApplicationBuilder> UseMoviesPersistenceEfAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();

        if (env.IsDevelopment())
        {
            var dbMntcService = scope.ServiceProvider.GetRequiredService<IDbMntcService>();
            await dbMntcService.MigrateAsync();
        }

        return app;
    }

}//Cls
