namespace HumanEmotionApi.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IHumanRepository Humans { get; }
    IEmotionRepository Emotions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

