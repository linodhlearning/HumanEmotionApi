using HumanEmotionApi.Domain.Entities;

namespace HumanEmotionApi.Application.DTOs;

public record HumanDto(
    Guid Id,
    string Name,
    string ShowColor,
    double EmotionScorePercentage,
    IReadOnlyList<EmotionDto> Emotions,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
)
{
    public static HumanDto FromEntity(Human human) =>
        new(
            human.Id,
            human.Name,
            human.ShowColor,
            Math.Round(human.EmotionScorePercentage, 2),
            human.Emotions.Select(EmotionDto.FromEntity).ToList().AsReadOnly(),
            human.CreatedAtUtc,
            human.UpdatedAtUtc
        );
}

