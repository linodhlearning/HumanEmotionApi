using MediatR;
using HumanEmotionApi.Application.Interfaces;
using HumanEmotionApi.Domain.Entities;
using HumanEmotionApi.Domain.Enums;
using HumanEmotionApi.Domain.Exceptions;

namespace HumanEmotionApi.Application.Emotions.Commands;

public record DeleteEmotionCommand(Guid HumanId, EmotionType Type) : IRequest<bool>;

public class DeleteEmotionCommandHandler : IRequestHandler<DeleteEmotionCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmotionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteEmotionCommand request, CancellationToken cancellationToken)
    {
        var human = await _unitOfWork.Humans.GetByIdWithEmotionsAsync(request.HumanId, cancellationToken);
        if (human == null)
        {
            throw new NotFoundException(nameof(Human), request.HumanId);
        }

        var existing = human.Emotions.FirstOrDefault(e => e.Type == request.Type);
        if (existing == null)
        {
            throw new NotFoundException($"Emotion '{request.Type}' was not found on human '{request.HumanId}'.");
        }

        human.RemoveEmotion(request.Type);
        _unitOfWork.Emotions.Delete(existing);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
