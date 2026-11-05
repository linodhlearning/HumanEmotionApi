using MediatR;
using HumanEmotionApi.Application.Interfaces;
using HumanEmotionApi.Domain.Exceptions;

namespace HumanEmotionApi.Application.Humans.Commands;

public record DeleteHumanCommand(Guid Id) : IRequest<bool>;

public class DeleteHumanCommandHandler : IRequestHandler<DeleteHumanCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteHumanCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteHumanCommand request, CancellationToken cancellationToken)
    {
        var human = await _unitOfWork.Humans.GetByIdAsync(request.Id, cancellationToken);
        if (human == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Human), request.Id);
        }

        _unitOfWork.Humans.Delete(human);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

