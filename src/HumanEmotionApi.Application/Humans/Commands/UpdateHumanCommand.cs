using MediatR;
using HumanEmotionApi.Application.DTOs;
using HumanEmotionApi.Application.Interfaces;
using HumanEmotionApi.Domain.Exceptions;

namespace HumanEmotionApi.Application.Humans.Commands;

public record UpdateHumanCommand(Guid Id, string Name) : IRequest<HumanDto>;

public class UpdateHumanCommandHandler : IRequestHandler<UpdateHumanCommand, HumanDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateHumanCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<HumanDto> Handle(UpdateHumanCommand request, CancellationToken cancellationToken)
    {
        var human = await _unitOfWork.Humans.GetByIdWithEmotionsAsync(request.Id, cancellationToken);
        if (human == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Human), request.Id);
        }

        human.UpdateName(request.Name);
        _unitOfWork.Humans.Update(human);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return HumanDto.FromEntity(human);
    }
}

