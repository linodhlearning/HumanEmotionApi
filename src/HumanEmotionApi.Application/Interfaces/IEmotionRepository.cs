using HumanEmotionApi.Domain.Entities;
using HumanEmotionApi.Domain.Enums;

namespace HumanEmotionApi.Application.Interfaces;

public interface IEmotionRepository : IRepository<Emotion>
{
    Task<Emotion?> GetByHumanAndTypeAsync(Guid humanId, EmotionType type, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Emotion>> GetByHumanIdAsync(Guid humanId, CancellationToken cancellationToken = default);
}

