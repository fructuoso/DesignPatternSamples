using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Xunit;
using CachingAbstractions = Microsoft.Extensions.Caching.Distributed;

namespace Workbench.IDistributedCache.Extensions.Tests;

public class IDistributedCacheExtensionsTests
{
    [Serializable]
    private class TestData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private readonly CachingAbstractions.IDistributedCache _cache;

    public IDistributedCacheExtensionsTests()
    {
        var opts = Options.Create(new MemoryDistributedCacheOptions());
        _cache = new MemoryDistributedCache(opts);
    }

    [Fact(DisplayName = "Set e Get - Deve armazenar e recuperar objeto corretamente")]
    public void SetAndGet_DeveArmazenarERecuperarObjetoCorretamente()
    {
        // Arrange
        var testData = new TestData { Id = 1, Name = "Test" };
        var key = "test-key";
        var ttl = 60;

        // Act
        _cache.Set(key, testData, ttl);
        var result = _cache.Get<TestData>(key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test", result.Name);
    }

    [Fact(DisplayName = "SetAsync e GetAsync - Deve armazenar e recuperar objeto assíncronamente")]
    public async Task SetAsyncAndGetAsync_DeveArmazenarERecuperarObjetoAsync()
    {
        // Arrange
        var testData = new TestData { Id = 2, Name = "Async Test" };
        var key = "async-key";
        var ttl = 120;

        // Act
        await _cache.SetAsync(key, testData, ttl);
        var result = await _cache.GetAsync<TestData>(key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Id);
        Assert.Equal("Async Test", result.Name);
    }

    [Fact(DisplayName = "Get - Deve retornar default quando não existe no cache")]
    public void Get_DeveRetornarDefault_QuandoNaoExisteNoCache()
    {
        // Arrange
        var key = "non-existent";

        // Act
        var result = _cache.Get<TestData>(key);

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "GetAsync - Deve retornar default quando não existe no cache")]
    public async Task GetAsync_DeveRetornarDefault_QuandoNaoExisteNoCache()
    {
        // Arrange
        var key = "non-existent-async";

        // Act
        var result = await _cache.GetAsync<TestData>(key);

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "GetOrCreate - Deve retornar do cache quando existe")]
    public void GetOrCreate_DeveRetornarDoCache_QuandoExiste()
    {
        // Arrange
        var cachedData = new TestData { Id = 5, Name = "From Cache" };
        var key = "get-or-create-key";
        var predicateCalled = false;

        _cache.Set(key, cachedData, 60);

        // Act
        var result = _cache.GetOrCreate(key, () =>
        {
            predicateCalled = true;
            return new TestData { Id = 999, Name = "Should not be called" };
        }, 60);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
        Assert.Equal("From Cache", result.Name);
        Assert.False(predicateCalled);
    }

    [Fact(DisplayName = "GetOrCreate - Deve criar e armazenar quando não existe no cache")]
    public void GetOrCreate_DeveCriarEArmazenar_QuandoNaoExisteNoCache()
    {
        // Arrange
        var key = "new-key";
        var newData = new TestData { Id = 6, Name = "Newly Created" };
        
        // Act
        var result = _cache.GetOrCreate(key, () => newData, 60);
        var cachedResult = _cache.Get<TestData>(key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(6, result.Id);
        Assert.Equal("Newly Created", result.Name);
        
        Assert.NotNull(cachedResult);
        Assert.Equal(6, cachedResult.Id);
    }

    [Fact(DisplayName = "GetOrCreateAsync - Deve retornar do cache quando existe")]
    public async Task GetOrCreateAsync_DeveRetornarDoCache_QuandoExiste()
    {
        // Arrange
        var cachedData = new TestData { Id = 7, Name = "Async From Cache" };
        var key = "async-get-or-create-key";
        var predicateCalled = false;

        await _cache.SetAsync(key, cachedData, 60);

        // Act
        var result = await _cache.GetOrCreateAsync(key, async () =>
        {
            predicateCalled = true;
            await Task.Delay(1);
            return new TestData { Id = 999, Name = "Should not be called" };
        }, 60);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.Id);
        Assert.Equal("Async From Cache", result.Name);
        Assert.False(predicateCalled);
    }

    [Fact(DisplayName = "GetOrCreateAsync - Deve criar e armazenar assincronamente quando não existe")]
    public async Task GetOrCreateAsync_DeveCriarEArmazenarAsync_QuandoNaoExiste()
    {
        // Arrange
        var key = "async-new-key";
        var newData = new TestData { Id = 8, Name = "Async Created" };
        
        // Act
        var result = await _cache.GetOrCreateAsync(key, async () =>
        {
            await Task.Delay(1);
            return newData;
        }, 120);
        var cachedResult = await _cache.GetAsync<TestData>(key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(8, result.Id);
        Assert.Equal("Async Created", result.Name);
        
        Assert.NotNull(cachedResult);
        Assert.Equal(8, cachedResult.Id);
    }

    [Theory(DisplayName = "Set - Deve suportar diferentes tipos de dados")]
    [InlineData(10, "Data Type 1")]
    [InlineData(20, "Data Type 2")]
    [InlineData(30, "Data Type 3")]
    public void Set_DeveSuportarDiferentesTiposDeDados(int id, string name)
    {
        // Arrange
        var testData = new TestData { Id = id, Name = name };
        var key = $"test-{id}";

        // Act
        _cache.Set(key, testData, 60);
        var result = _cache.Get<TestData>(key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(name, result.Name);
    }
}
