using HumanEmotionApi.Application.Interfaces;
using HumanEmotionApi.Domain.Entities;
using HumanEmotionApi.Domain.Enums;
using HumanEmotionApi.Infrastructure;
using HumanEmotionApi.Infrastructure.Data;
using HumanEmotionApi.Infrastructure.Repositories;
using HumanEmotionApi.Infrastructure.UnitOfWork;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HumanEmotionApi.Tests.Infrastructure;

public class RepositoryAndUnitOfWorkTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<HumanEmotionDbContext> _options;

    public RepositoryAndUnitOfWorkTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<HumanEmotionDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new HumanEmotionDbContext(_options);
        context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task HumanRepository_AddAndGetByIdWithEmotions_ShouldPersistAndLoadEmotions()
    {
        using var context = new HumanEmotionDbContext(_options);
        var uow = new UnitOfWork(context);

        var human = new Human("Charlie");
        human.AddEmotion(EmotionType.Joy, 90.0);
        human.AddEmotion(EmotionType.Fear, 40.0);

        await uow.Humans.AddAsync(human);
        await uow.SaveChangesAsync();

        using var assertContext = new HumanEmotionDbContext(_options);
        var assertUow = new UnitOfWork(assertContext);

        var retrieved = await assertUow.Humans.GetByIdWithEmotionsAsync(human.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("Charlie", retrieved.Name);
        Assert.Equal(2, retrieved.Emotions.Count);
        Assert.True(retrieved.HasEmotion(EmotionType.Joy));
        Assert.True(retrieved.HasEmotion(EmotionType.Fear));
        Assert.NotEqual("#FFFFFF", retrieved.ShowColor);
    }

    [Fact]
    public async Task HumanRepository_GetAllWithEmotionsAsync_ShouldReturnAllHumans()
    {
        using var context = new HumanEmotionDbContext(_options);
        var uow = new UnitOfWork(context);

        var h1 = new Human("Person 1");
        var h2 = new Human("Person 2");
        await uow.Humans.AddAsync(h1);
        await uow.Humans.AddAsync(h2);
        await uow.SaveChangesAsync();

        using var assertContext = new HumanEmotionDbContext(_options);
        var assertUow = new UnitOfWork(assertContext);

        var all = await assertUow.Humans.GetAllWithEmotionsAsync();
        Assert.True(all.Count >= 2);
    }

    [Fact]
    public async Task HumanRepository_ExistsAsync_ShouldReturnCorrectBoolean()
    {
        using var context = new HumanEmotionDbContext(_options);
        var uow = new UnitOfWork(context);

        var human = new Human("David");
        await uow.Humans.AddAsync(human);
        await uow.SaveChangesAsync();

        Assert.True(await uow.Humans.ExistsAsync(human.Id));
        Assert.False(await uow.Humans.ExistsAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task HumanRepository_Delete_ShouldCascadeDeleteEmotions()
    {
        using var context = new HumanEmotionDbContext(_options);
        var uow = new UnitOfWork(context);

        var human = new Human("Eva");
        human.AddEmotion(EmotionType.Anger, 70.0);
        await uow.Humans.AddAsync(human);
        await uow.SaveChangesAsync();

        uow.Humans.Delete(human);
        await uow.SaveChangesAsync();

        using var assertContext = new HumanEmotionDbContext(_options);
        var assertUow = new UnitOfWork(assertContext);

        var retrievedHuman = await assertUow.Humans.GetByIdAsync(human.Id);
        var emotions = await assertUow.Emotions.GetByHumanIdAsync(human.Id);

        Assert.Null(retrievedHuman);
        Assert.Empty(emotions);
    }

    [Fact]
    public async Task EmotionRepository_GetByHumanAndTypeAsync_ShouldReturnMatchingEmotion()
    {
        using var context = new HumanEmotionDbContext(_options);
        var uow = new UnitOfWork(context);

        var human = new Human("Frank");
        human.AddEmotion(EmotionType.Sadness, 65.0);
        await uow.Humans.AddAsync(human);
        await uow.SaveChangesAsync();

        using var assertContext = new HumanEmotionDbContext(_options);
        var assertUow = new UnitOfWork(assertContext);

        var emotion = await assertUow.Emotions.GetByHumanAndTypeAsync(human.Id, EmotionType.Sadness);
        Assert.NotNull(emotion);
        Assert.Equal(65.0, emotion.Percentage);

        var notFound = await assertUow.Emotions.GetByHumanAndTypeAsync(human.Id, EmotionType.Anger);
        Assert.Null(notFound);
    }

    [Fact]
    public async Task GenericRepository_GetAllAndGetByIdAsync_ShouldWork()
    {
        using var context = new HumanEmotionDbContext(_options);
        var repo = new Repository<Human>(context);

        var human = new Human("Grace");
        await repo.AddAsync(human);
        await context.SaveChangesAsync();

        var retrieved = await repo.GetByIdAsync(human.Id);
        Assert.NotNull(retrieved);

        var all = await repo.GetAllAsync();
        Assert.Contains(all, h => h.Id == human.Id);
    }

    [Fact]
    public void UnitOfWork_Dispose_CanBeCalledMultipleTimesWithoutThrowing()
    {
        var context = new HumanEmotionDbContext(_options);
        var uow = new UnitOfWork(context);

        uow.Dispose();
        uow.Dispose();
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterServices()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddInfrastructure(configuration);
        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<HumanEmotionDbContext>());
        Assert.NotNull(provider.GetService<IHumanRepository>());
        Assert.NotNull(provider.GetService<IEmotionRepository>());
        Assert.NotNull(provider.GetService<IUnitOfWork>());
    }
}

