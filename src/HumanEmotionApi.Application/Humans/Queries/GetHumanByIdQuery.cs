using MediatR;
using HumanEmotionApi.Application.DTOs;
using HumanEmotionApi.Application.Interfaces;

namespace HumanEmotionApi.Application.Humans.Queries;

public record GetHumanByIdQuery(Guid Id) : IRequest<HumanDto?>;

public class GetHumanByIdQueryHandler : IRequestHandler<GetHumanByIdQuery, HumanDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetHumanByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<HumanDto?> Handle(GetHumanByIdQuery request, CancellationToken cancellationToken)
    {
        var human = await _unitOfWork.Humans.GetByIdWithEmotionsAsync(request.Id, cancellationToken);
        return human == null ? null : HumanDto.FromEntity(human);
    }
}

