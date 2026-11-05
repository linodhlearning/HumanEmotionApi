using MediatR;
using HumanEmotionApi.Application.DTOs;
using HumanEmotionApi.Application.Interfaces;

namespace HumanEmotionApi.Application.Humans.Queries;

public record GetAllHumansQuery : IRequest<IReadOnlyList<HumanDto>>;

public class GetAllHumansQueryHandler : IRequestHandler<GetAllHumansQuery, IReadOnlyList<HumanDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllHumansQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<HumanDto>> Handle(GetAllHumansQuery request, CancellationToken cancellationToken)
    {
        var humans = await _unitOfWork.Humans.GetAllWithEmotionsAsync(cancellationToken);
        return humans.Select(HumanDto.FromEntity).ToList().AsReadOnly();
    }
}

