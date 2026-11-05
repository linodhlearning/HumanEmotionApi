using HumanEmotionApi.Application.Interfaces;
using HumanEmotionApi.Domain.Entities;
using HumanEmotionApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HumanEmotionApi.Infrastructure.Repositories;

public class HumanRepository : Repository<Human>, IHumanRepository
{
    public HumanRepository(HumanEmotionDbContext context) : base(context)
    {
    }

    public async Task<Human?> GetByIdWithEmotionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(h => h.Emotions)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Human>> GetAllWithEmotionsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(h => h.Emotions)
            .OrderByDescending(h => h.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(h => h.Id == id, cancellationToken);
    }
}

