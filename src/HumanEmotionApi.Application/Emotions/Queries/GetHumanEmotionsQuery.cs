using MediatR;
using HumanEmotionApi.Application.DTOs;
using HumanEmotionApi.Application.Interfaces;
using HumanEmotionApi.Domain.Entities;
using HumanEmotionApi.Domain.Exceptions;

namespace HumanEmotionApi.Application.Emotions.Queries;

public record GetHumanEmotionsQuery(Guid HumanId) : IRequest<IReadOnlyList<EmotionDto>>;

public class GetHumanEmotionsQueryHandler : IRequestHandler<GetHumanEmotionsQuery, IReadOnlyList<EmotionDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetHumanEmotionsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<EmotionDto>> Handle(GetHumanEmotionsQuery request, CancellationToken cancellationToken)
    {
        var human = await _unitOfWork.Humans.GetByIdWithEmotionsAsync(request.HumanId, cancellationToken);
        if (human == null)
        {
            throw new NotFoundException(nameof(Human), request.HumanId);
        }

        return human.Emotions.Select(EmotionDto.FromEntity).ToList().AsReadOnly();
    }
}

