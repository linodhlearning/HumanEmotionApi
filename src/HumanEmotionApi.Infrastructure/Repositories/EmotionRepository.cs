using HumanEmotionApi.Application.Interfaces;
using HumanEmotionApi.Domain.Entities;
using HumanEmotionApi.Domain.Enums;
using HumanEmotionApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HumanEmotionApi.Infrastructure.Repositories;

public class EmotionRepository : Repository<Emotion>, IEmotionRepository
{
    public EmotionRepository(HumanEmotionDbContext context) : base(context)
    {
    }

    public async Task<Emotion?> GetByHumanAndTypeAsync(Guid humanId, EmotionType type, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(e => e.HumanId == humanId && e.Type == type, cancellationToken);
    }

    public async Task<IReadOnlyList<Emotion>> GetByHumanIdAsync(Guid humanId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(e => e.HumanId == humanId)
            .OrderBy(e => e.Type)
            .ToListAsync(cancellationToken);
    }
}

