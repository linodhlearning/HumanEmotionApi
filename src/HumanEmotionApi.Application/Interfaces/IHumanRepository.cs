using HumanEmotionApi.Domain.Entities;

namespace HumanEmotionApi.Application.Interfaces;

public interface IHumanRepository : IRepository<Human>
{
    Task<Human?> GetByIdWithEmotionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Human>> GetAllWithEmotionsAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}

