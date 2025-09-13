using Microsoft.Extensions.Caching.Distributed;
using Movies.Shared.Models;
using Movies.Shared.Repos;
using System.Text.Json;

namespace Movies.Persistence.Cache.Redis.Repos;
internal class MoviesRepoRedisCached(IMoviesRepo _repo, IDistributedCache _cache)
    : IMoviesRepo
{
    private const string _allCacheKey = "Movies_All";
    private static string IdCacheKey(int id) => $"Movies_Id_{id}";

    private static readonly DistributedCacheEntryOptions _defaultCacheOptions = new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(15)
    };

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    //--------------------------//

    public async Task<Movie> AddAsync(Movie entity, CancellationToken cancellationToken = default)
    {
        var added = await _repo.AddAsync(entity, cancellationToken);
        // Invalidate caches
        await _cache.RemoveAsync(_allCacheKey, cancellationToken);
        await _cache.RemoveAsync(IdCacheKey(added.Id), cancellationToken);
        return added;
    }

    //--------------------------//

    public async Task AddRangeAsync(IEnumerable<Movie> entities, CancellationToken cancellationToken = default)
    {
        await _repo.AddRangeAsync(entities, cancellationToken);
        await _cache.RemoveAsync(_allCacheKey, cancellationToken);
        // Remove any per-id entries for provided entities if they already have ids
        foreach (var e in entities)
        {
            if (e is not null && e.Id != 0)
                await _cache.RemoveAsync(IdCacheKey(e.Id), cancellationToken);
        }
    }

    //--------------------------//

    public async Task<int> CountAsync()
    {
        // Try to use cached list
        string? cachedListValue = await _cache.GetStringAsync(_allCacheKey);

        if (string.IsNullOrWhiteSpace(cachedListValue))
            return await _repo.CountAsync(); //Go with DB value

        return JsonSerializer.Deserialize<List<Movie>>(cachedListValue, _jsonOptions)
            ?.Count
            ?? 0;
    }

    //--------------------------//

    public async Task DeleteAsync(Movie entity)
    {
        if (entity is null)
            return;

        await _repo.DeleteAsync(entity);
        await _cache.RemoveAsync(_allCacheKey);
        if (entity.Id != 0)
            await _cache.RemoveAsync(IdCacheKey(entity.Id));
    }

    //--------------------------//

    public async Task DeleteAsync(int? id)
    {
        if (!id.HasValue)
            return;

        await _repo.DeleteAsync(id);
        await _cache.RemoveAsync(_allCacheKey);
        await _cache.RemoveAsync(IdCacheKey(id.Value));
    }

    //--------------------------//

    public async Task<bool> ExistsAsync(int? id)
    {
        if (!id.HasValue)
            return false;


        // Try by id cache
        var key = IdCacheKey(id.Value);
        string? cachedEntity = await _cache.GetStringAsync(key);

        if (cachedEntity is not null)
            return true;

        var entity = await _repo.FirstOrDefaultByIdAsync(id);
        if (entity is null)
            return false;

        await _cache.SetStringAsync(key, JsonSerializer.Serialize(entity, _jsonOptions), _defaultCacheOptions);


        return true;
    }

    //--------------------------//

    public async Task<Movie?> FirstOrDefaultByIdAsync(int? id)
    {
        if (!id.HasValue)
            return null;

        var key = IdCacheKey(id.Value);
        string? cachedEntity = await _cache.GetStringAsync(key);

        if (!string.IsNullOrWhiteSpace(cachedEntity))
        {
            await _cache.RefreshAsync(key); // renew sliding expiration
            // Deserialize the cached value back to a Movie object
            return JsonSerializer.Deserialize<Movie>(cachedEntity, _jsonOptions);
        }

        var entity = await _repo.FirstOrDefaultByIdAsync(id);

        if (entity is not null)
            //Save value for next time
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(entity, _jsonOptions), _defaultCacheOptions);

        return entity;
    }

    //--------------------------//

    public async Task<IReadOnlyList<Movie>> ListAllAsync()
    {
        string? cachedListValue = await _cache.GetStringAsync(_allCacheKey);

        if (!string.IsNullOrWhiteSpace(cachedListValue))
        {
            var deserialized = JsonSerializer.Deserialize<List<Movie>>(cachedListValue, _jsonOptions) ?? [];
            return deserialized;
        }

        var entities = await _repo.ListAllAsync();
        await _cache.SetStringAsync(_allCacheKey, JsonSerializer.Serialize(entities, _jsonOptions), _defaultCacheOptions);

        // populate individual id entries for faster single lookups
        foreach (var entity in entities)
            await _cache.SetStringAsync(IdCacheKey(entity.Id), JsonSerializer.Serialize(entity, _jsonOptions), _defaultCacheOptions);

        return entities;
    }

    //--------------------------//

    public async Task<IReadOnlyList<Movie>> ListByIdsAsync(IEnumerable<int>? ids)
    {
        if (ids == null)
            return [];

        string? cachedListValue = await _cache.GetStringAsync(_allCacheKey);

        var idList = ids.ToList();
        // Try to satisfy from cached all
        if (!string.IsNullOrWhiteSpace(cachedListValue))
        {
            var idSet = idList.ToHashSet();
            var all = JsonSerializer.Deserialize<List<Movie>>(cachedListValue, _jsonOptions) ?? [];
            var found = all?.Where(m => idSet.Contains(m.Id)).ToList();
            if (found?.Count == idSet.Count)
                return found;
        }

        var entities = await _repo.ListByIdsAsync(ids);
        // Cache individual items
        foreach (var entity in entities)
            await _cache.SetStringAsync(IdCacheKey(entity.Id), JsonSerializer.Serialize(entity, _jsonOptions), _defaultCacheOptions);

        return entities;
    }

    //--------------------------//

    public async Task RemoveRangeAsync(IEnumerable<Movie> entities)
    {
        await _repo.RemoveRangeAsync(entities);
        await _cache.RemoveAsync(_allCacheKey);
        foreach (var e in entities)
            await _cache.RemoveAsync(IdCacheKey(e.Id));
    }

    //--------------------------//

    public async Task<IReadOnlyList<Movie>> TakeAsync(int count, int skip, CancellationToken cancellationToken = default) =>
        // Not caching paged results; delegate
        await _repo.TakeAsync(count, skip, cancellationToken);

    //--------------------------//

    public async Task<Movie> UpdateAsync(Movie entity)
    {
        var updated = await _repo.UpdateAsync(entity);
        await _cache.RemoveAsync(_allCacheKey);
        await _cache.RemoveAsync(IdCacheKey(entity.Id));
        return updated;
    }

    //--------------------------//

    public async Task UpdateRangeAsync(IEnumerable<Movie> entities)
    {
        await _repo.UpdateRangeAsync(entities);
        await _cache.RemoveAsync(_allCacheKey);
        foreach (var e in entities)
            await _cache.RemoveAsync(IdCacheKey(e.Id));
    }

}
