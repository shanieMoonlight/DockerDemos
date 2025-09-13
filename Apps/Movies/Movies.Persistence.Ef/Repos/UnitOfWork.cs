using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Movies.Shared.Repos;
using NeoSmart.AsyncLock;
using System.Diagnostics;

namespace Movies.Persistence.Ef.Repos;
internal class MoviesUnitOfWork(
    MoviesDbContext db,
    IMoviesRepo moviesInfo
    ) : IMoviesUnitOfWork
{

    private static readonly AsyncLock _asyncLock = new();
    private static readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    //--------------------------// 

    public IMoviesRepo MoviesRepo => moviesInfo;

    //--------------------------// 

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    //--------------------------// 

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await db.SaveChangesAsync(cancellationToken);

    //--------------------------// 

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken) =>
        await db.Database.BeginTransactionAsync(cancellationToken);

    //--------------------------// 

    public IExecutionStrategy CreateExecutionStrategy() =>
                db.Database.CreateExecutionStrategy();

    //--------------------------// 

    /// <summary>
    /// Executes <paramref name="toExecute"/> in a transaction and automatically rolls it back if any exceptions are thrown 
    /// Otherwise it SaveChanges and Commits 
    /// </summary>
    /// <typeparam name="ReturnType"></typeparam>
    /// <param name="toExecute">The Code you want to run in the transaction</param>
    /// <param name="onError">What to do with Exceptions. Will just throw if not sullpied</param>
    public async Task<ReturnType> ExecuteAsyncTransaction<ReturnType>(
        Func<Task<ReturnType>> toExecute,
        Func<Exception, ReturnType>? onError,
        CancellationToken cancellationToken)
    {

        await _semaphoreSlim.WaitAsync(cancellationToken);
        try
        {
            if (db.Database.CurrentTransaction is not null)
                return await toExecute();

            var strategy = CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await BeginTransactionAsync(cancellationToken);
                return await ExecuteTransaction(transaction, toExecute, onError, cancellationToken);
            });
        }
        catch (Exception e)
        {
            Debug.WriteLine("DEBBUMoviesING............\r\n" + e.Message + "\r\n" + e.StackTrace);
            throw;
        }
        finally
        {
            _semaphoreSlim.Release();

        }
    }


    //--------------------------// 

    private async Task<ReturnType> ExecuteTransaction<ReturnType>(
        IDbContextTransaction transaction,
        Func<Task<ReturnType>> toExecute,
        Func<Exception, ReturnType>? onError,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await toExecute();

            await SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            if (onError is null)
                throw;
            return onError(e);
        }
    }

    //--------------------------// 


}//Cls
