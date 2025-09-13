using Microsoft.Extensions.Caching.Memory;
using Movies.Shared.Models;
using Movies.Shared.Repos;

namespace Movies.Persistence.Cache.Memory.Repos;
internal class MoviesRepoMemoryCached(IMoviesRepo repo, IMemoryCache cache)
    : IMoviesRepo
{
    private readonly IMoviesRepo _repo = repo;
    private readonly IMemoryCache _cache = cache;

    private const string _allCacheKey = "Movies_All";
    private static string IdCacheKey(int id) => $"Movies_Id_{id}";

    private static MemoryCacheEntryOptions DefaultCacheOptions() =>
        new()
        {
            SlidingExpiration = TimeSpan.FromMinutes(15)
        };

    //--------------------------//

    public async Task<Movie> AddAsync(Movie entity, CancellationToken cancellationToken = default)
    {
        var added = await _repo.AddAsync(entity, cancellationToken);
        // Invalidate caches
        _cache.Remove(_allCacheKey);
        _cache.Remove(IdCacheKey(added.Id));
        return added;
    }

    //--------------------------//

    public async Task AddRangeAsync(IEnumerable<Movie> entities, CancellationToken cancellationToken = default)
    {
        await _repo.AddRangeAsync(entities, cancellationToken);
        _cache.Remove(_allCacheKey);
        // Remove any per-id entries for provided entities if they already have ids
        foreach (var e in entities)
        {
            if (e is not null && e.Id != 0)
                _cache.Remove(IdCacheKey(e.Id));
        }
    }

    //--------------------------//

    public async Task<int> CountAsync()
    {
        // Try to use cached list
        if (_cache.TryGetValue<IReadOnlyList<Movie>>(_allCacheKey, out var list))
            return list?.Count ?? 0;

        return await _repo.CountAsync();
    }

    //--------------------------//

    public async Task DeleteAsync(Movie entity)
    {
        if (entity is null)
            return;

        await _repo.DeleteAsync(entity);
        _cache.Remove(_allCacheKey);
        if (entity.Id != 0)
            _cache.Remove(IdCacheKey(entity.Id));
    }

    //--------------------------//

    public async Task DeleteAsync(int? id)
    {
        if (!id.HasValue)
            return;

        await _repo.DeleteAsync(id);
        _cache.Remove(_allCacheKey);
        _cache.Remove(IdCacheKey(id.Value));
    }

    //--------------------------//

    public async Task<bool> ExistsAsync(int? id)
    {
        if (!id.HasValue)
            return false;

        // Try by id cache
        if (_cache.TryGetValue<Movie?>(IdCacheKey(id.Value), out var maybe))
            return maybe is not null;

        var exists = await _repo.ExistsAsync(id);
        if (exists)
        {
            // If exists, populate id cache with the entity for faster subsequent access
            var entity = await _repo.FirstOrDefaultByIdAsync(id);
            _cache.Set(IdCacheKey(id.Value), entity, DefaultCacheOptions());
        }

        return exists;
    }

    //--------------------------//

    public async Task<Movie?> FirstOrDefaultByIdAsync(int? id)
    {
        if (!id.HasValue)
            return null;

        var key = IdCacheKey(id.Value);
        return await _cache.GetOrCreateAsync(
                key,
                async entry =>
                {
                    entry.SetOptions(DefaultCacheOptions());
                    var item = await _repo.FirstOrDefaultByIdAsync(id);
                    return item;
                });
    }

    //--------------------------//

    public async Task<IReadOnlyList<Movie>> ListAllAsync()
    {
        if (_cache.TryGetValue<IReadOnlyList<Movie>>(_allCacheKey, out var list))
            return list ?? [];

        var result = await _repo.ListAllAsync();
        _cache.Set(_allCacheKey, result, DefaultCacheOptions());

        // populate individual id entries for faster single lookups
        foreach (var m in result)
            _cache.Set(IdCacheKey(m.Id), m, DefaultCacheOptions());

        return result;
    }

    //--------------------------//

    public async Task<IReadOnlyList<Movie>> ListByIdsAsync(IEnumerable<int>? ids)
    {
        if (ids == null)
            return [];

        var idList = ids.ToList();
        // Try to satisfy from cached all
        if (_cache.TryGetValue<IReadOnlyList<Movie>>(_allCacheKey, out var all))
        {
            var idSet = idList.ToHashSet();
            var found = all?.Where(m => idSet.Contains(m.Id)).ToList();
            if (found?.Count == idSet.Count)
                return found;
            // otherwise fallthrough to repo
        }

        var result = await _repo.ListByIdsAsync(ids);
        // Cache individual items
        foreach (var m in result)
            _cache.Set(IdCacheKey(m.Id), m, DefaultCacheOptions());

        return result;
    }

    //--------------------------//

    public async Task RemoveRangeAsync(IEnumerable<Movie> entities)
    {
        await _repo.RemoveRangeAsync(entities);
        _cache.Remove(_allCacheKey);
        foreach (var e in entities)
            _cache.Remove(IdCacheKey(e.Id));
    }

    //--------------------------//

    public async Task<IReadOnlyList<Movie>> TakeAsync(int count, int skip, CancellationToken cancellationToken = default) =>
        // Not caching paged results; delegate
        await _repo.TakeAsync(count, skip, cancellationToken);

    //--------------------------//

    public async Task<Movie> UpdateAsync(Movie entity)
    {
        var updated = await _repo.UpdateAsync(entity);
        _cache.Remove(_allCacheKey);
        _cache.Remove(IdCacheKey(entity.Id));
        return updated;
    }

    //--------------------------//

    public async Task UpdateRangeAsync(IEnumerable<Movie> entities)
    {
        await _repo.UpdateRangeAsync(entities);
        _cache.Remove(_allCacheKey);
        foreach (var e in entities)
            _cache.Remove(IdCacheKey(e.Id));
    }

    //--------------------------//

}
