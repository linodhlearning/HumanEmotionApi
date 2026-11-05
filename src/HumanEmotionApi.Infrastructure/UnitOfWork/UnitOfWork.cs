using HumanEmotionApi.Application.Interfaces;
using HumanEmotionApi.Infrastructure.Data;
using HumanEmotionApi.Infrastructure.Repositories;

namespace HumanEmotionApi.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly HumanEmotionDbContext _context;
    private IHumanRepository? _humans;
    private IEmotionRepository? _emotions;
    private bool _disposed;

    public UnitOfWork(HumanEmotionDbContext context)
    {
        _context = context;
    }

    public IHumanRepository Humans => _humans ??= new HumanRepository(_context);
    public IEmotionRepository Emotions => _emotions ??= new EmotionRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}

