using MediatR;
using HumanEmotionApi.Application.DTOs;
using HumanEmotionApi.Application.Interfaces;
using HumanEmotionApi.Domain.Entities;
using HumanEmotionApi.Domain.Enums;
using HumanEmotionApi.Domain.Exceptions;

namespace HumanEmotionApi.Application.Emotions.Queries;

public record GetEmotionByTypeQuery(Guid HumanId, EmotionType Type) : IRequest<EmotionDto?>;

public class GetEmotionByTypeQueryHandler : IRequestHandler<GetEmotionByTypeQuery, EmotionDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEmotionByTypeQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<EmotionDto?> Handle(GetEmotionByTypeQuery request, CancellationToken cancellationToken)
    {
        var human = await _unitOfWork.Humans.GetByIdWithEmotionsAsync(request.HumanId, cancellationToken);
        if (human == null)
        {
            throw new NotFoundException(nameof(Human), request.HumanId);
        }

        var emotion = human.Emotions.FirstOrDefault(e => e.Type == request.Type);
        return emotion == null ? null : EmotionDto.FromEntity(emotion);
    }
}

