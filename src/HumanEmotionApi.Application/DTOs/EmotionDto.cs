using HumanEmotionApi.Domain.Entities;

namespace HumanEmotionApi.Application.DTOs;

public record EmotionDto(
    Guid Id,
    Guid HumanId,
    string Type,
    double Percentage,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
)
{
    public static EmotionDto FromEntity(Emotion emotion) =>
        new(
            emotion.Id,
            emotion.HumanId,
            emotion.Type.ToString(),
            emotion.Percentage,
            emotion.CreatedAtUtc,
            emotion.UpdatedAtUtc
        );
}

