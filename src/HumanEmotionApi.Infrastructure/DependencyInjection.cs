using HumanEmotionApi.Application.Interfaces;
using HumanEmotionApi.Infrastructure.Data;
using HumanEmotionApi.Infrastructure.Repositories;
using HumanEmotionApi.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HumanEmotionApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? "Data Source=human_emotions.db";

        services.AddDbContext<HumanEmotionDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IHumanRepository, HumanRepository>();
        services.AddScoped<IEmotionRepository, EmotionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

        return services;
    }
}

