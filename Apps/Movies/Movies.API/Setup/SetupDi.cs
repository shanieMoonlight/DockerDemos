namespace Movies.API.Setup;

public static class SetupDi
{
    public static IServiceCollection AddMoviesApiServices(this IServiceCollection services)
    {
        services.AddScoped<IMoviesService, MoviesService>();
        return services;
    }
}
