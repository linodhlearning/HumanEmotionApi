using MediatR;
using HumanEmotionApi.Application.DTOs;
using HumanEmotionApi.Application.Interfaces;
using HumanEmotionApi.Domain.Entities;
using HumanEmotionApi.Domain.Enums;
using HumanEmotionApi.Domain.Exceptions;

namespace HumanEmotionApi.Application.Emotions.Commands;

public record UpdateEmotionCommand(Guid HumanId, EmotionType Type, double Percentage) : IRequest<EmotionDto>;

public class UpdateEmotionCommandHandler : IRequestHandler<UpdateEmotionCommand, EmotionDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmotionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<EmotionDto> Handle(UpdateEmotionCommand request, CancellationToken cancellationToken)
    {
        var human = await _unitOfWork.Humans.GetByIdWithEmotionsAsync(request.HumanId, cancellationToken);
        if (human == null)
        {
            throw new NotFoundException(nameof(Human), request.HumanId);
        }

        var emotion = human.UpdateEmotion(request.Type, request.Percentage);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return EmotionDto.FromEntity(emotion);
    }
}

