using MediatR;
using HumanEmotionApi.Domain.Enums;

namespace HumanEmotionApi.Application.Emotions.Queries;

public record GetSupportedEmotionTypesQuery : IRequest<IReadOnlyList<string>>;

public class GetSupportedEmotionTypesQueryHandler : IRequestHandler<GetSupportedEmotionTypesQuery, IReadOnlyList<string>>
{
    public Task<IReadOnlyList<string>> Handle(GetSupportedEmotionTypesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<string> emotions = Enum.GetNames<EmotionType>().ToList().AsReadOnly();
        return Task.FromResult(emotions);
    }
}

