using System.Net;
using System.Net.Http.Json;
using HumanEmotionApi.Application.DTOs;
using HumanEmotionApi.Domain.Enums;
using HumanEmotionApi.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HumanEmotionApi.Tests.Api;

public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly SqliteConnection _sqliteConnection;

    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        _sqliteConnection = new SqliteConnection("DataSource=:memory:");
        _sqliteConnection.Open();

        var customFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext registration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<HumanEmotionDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory SQLite
                services.AddDbContext<HumanEmotionDbContext>(options =>
                {
                    options.UseSqlite(_sqliteConnection);
                });

                // Ensure schema created
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<HumanEmotionDbContext>();
                db.Database.EnsureCreated();
            });
        });

        _client = customFactory.CreateClient();
    }

    public void Dispose()
    {
        _sqliteConnection.Dispose();
        _client.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task EndToEnd_HumanEmotionLifecycle_ShouldWorkAsExpected()
    {
        // 1. Get supported emotion types
        var typesResponse = await _client.GetAsync("/api/emotions/types");
        Assert.Equal(HttpStatusCode.OK, typesResponse.StatusCode);
        var types = await typesResponse.Content.ReadFromJsonAsync<List<string>>();
        Assert.NotNull(types);
        Assert.Equal(5, types.Count);

        // 2. Create Human
        var createResponse = await _client.PostAsJsonAsync("/api/humans", new CreateHumanRequest("Dana"));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var human = await createResponse.Content.ReadFromJsonAsync<HumanDto>();
        Assert.NotNull(human);
        Assert.Equal("Dana", human.Name);
        Assert.Equal("#FFFFFF", human.ShowColor);

        var humanId = human.Id;

        // 3. Get Human by Id
        var getResponse = await _client.GetAsync($"/api/humans/{humanId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetched = await getResponse.Content.ReadFromJsonAsync<HumanDto>();
        Assert.NotNull(fetched);
        Assert.Equal(humanId, fetched.Id);

        // 4. Add Joy emotion with 100%
        var addEmotionResponse = await _client.PostAsJsonAsync($"/api/humans/{humanId}/emotions", new AddEmotionRequest(EmotionType.Joy, 100.0));
        Assert.Equal(HttpStatusCode.Created, addEmotionResponse.StatusCode);
        var emotion = await addEmotionResponse.Content.ReadFromJsonAsync<EmotionDto>();
        Assert.NotNull(emotion);
        Assert.Equal("Joy", emotion.Type);
        Assert.Equal(100.0, emotion.Percentage);

        // 5. Verify ShowColor changed from white towards green
        var updatedHumanResponse = await _client.GetAsync($"/api/humans/{humanId}");
        var updatedHuman = await updatedHumanResponse.Content.ReadFromJsonAsync<HumanDto>();
        Assert.NotNull(updatedHuman);
        Assert.Equal("#CCE6CC", updatedHuman.ShowColor);

        // 6. Get emotions for human
        var emotionsResponse = await _client.GetAsync($"/api/humans/{humanId}/emotions");
        Assert.Equal(HttpStatusCode.OK, emotionsResponse.StatusCode);
        var emotionsList = await emotionsResponse.Content.ReadFromJsonAsync<List<EmotionDto>>();
        Assert.NotNull(emotionsList);
        Assert.Single(emotionsList);

        // 7. Get specific emotion
        var singleEmotionResponse = await _client.GetAsync($"/api/humans/{humanId}/emotions/Joy");
        Assert.Equal(HttpStatusCode.OK, singleEmotionResponse.StatusCode);

        // 8. Update emotion percentage
        var updateEmotionResponse = await _client.PutAsJsonAsync($"/api/humans/{humanId}/emotions/Joy", new UpdateEmotionRequest(50.0));
        Assert.Equal(HttpStatusCode.OK, updateEmotionResponse.StatusCode);

        // 9. Delete emotion
        var deleteEmotionResponse = await _client.DeleteAsync($"/api/humans/{humanId}/emotions/Joy");
        Assert.Equal(HttpStatusCode.NoContent, deleteEmotionResponse.StatusCode);

        // 10. Update human name
        var updateHumanResponse = await _client.PutAsJsonAsync($"/api/humans/{humanId}", new UpdateHumanRequest("Dana Updated"));
        Assert.Equal(HttpStatusCode.OK, updateHumanResponse.StatusCode);

        // 11. Delete human
        var deleteHumanResponse = await _client.DeleteAsync($"/api/humans/{humanId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteHumanResponse.StatusCode);

        // 12. Verify deleted
        var getDeletedResponse = await _client.GetAsync($"/api/humans/{humanId}");
        Assert.Equal(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
    }
}
