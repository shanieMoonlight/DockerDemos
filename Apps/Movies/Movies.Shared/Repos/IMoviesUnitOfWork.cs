namespace Movies.Shared.Repos;
public interface IMoviesUnitOfWork : IDisposable
{
    IMoviesRepo MoviesRepo { get; }

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

}//int
