using MediatR;
using HumanEmotionApi.Application.DTOs;
using HumanEmotionApi.Application.Interfaces;
using HumanEmotionApi.Domain.Entities;

namespace HumanEmotionApi.Application.Humans.Commands;

public record CreateHumanCommand(string Name) : IRequest<HumanDto>;

public class CreateHumanCommandHandler : IRequestHandler<CreateHumanCommand, HumanDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateHumanCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<HumanDto> Handle(CreateHumanCommand request, CancellationToken cancellationToken)
    {
        var human = new Human(request.Name);
        await _unitOfWork.Humans.AddAsync(human, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return HumanDto.FromEntity(human);
    }
}

