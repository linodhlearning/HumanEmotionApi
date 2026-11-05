using MediatR;
using HumanEmotionApi.Application.DTOs;
using HumanEmotionApi.Application.Interfaces;
using HumanEmotionApi.Domain.Entities;
using HumanEmotionApi.Domain.Enums;
using HumanEmotionApi.Domain.Exceptions;

namespace HumanEmotionApi.Application.Emotions.Commands;

public record AddEmotionCommand(Guid HumanId, EmotionType Type, double Percentage) : IRequest<EmotionDto>;

public class AddEmotionCommandHandler : IRequestHandler<AddEmotionCommand, EmotionDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddEmotionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<EmotionDto> Handle(AddEmotionCommand request, CancellationToken cancellationToken)
    {
        var human = await _unitOfWork.Humans.GetByIdWithEmotionsAsync(request.HumanId, cancellationToken);
        if (human == null)
        {
            throw new NotFoundException(nameof(Human), request.HumanId);
        }

        var emotion = human.AddEmotion(request.Type, request.Percentage);
        await _unitOfWork.Emotions.AddAsync(emotion, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return EmotionDto.FromEntity(emotion);
    }
}
