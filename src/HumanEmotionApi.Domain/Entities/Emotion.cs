using HumanEmotionApi.Domain.Enums;
using HumanEmotionApi.Domain.Exceptions;

namespace HumanEmotionApi.Domain.Entities;

public class Emotion
{
    public Guid Id { get; private set; }
    public Guid HumanId { get; private set; }
    public EmotionType Type { get; private set; }
    public double Percentage { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    // EF Core constructor
    protected Emotion() { }

    public Emotion(Guid humanId, EmotionType type, double percentage)
    {
        if (humanId == Guid.Empty)
        {
            throw new DomainException("HumanId cannot be empty.");
        }

        if (!Enum.IsDefined(typeof(EmotionType), type))
        {
            throw new DomainException($"Invalid emotion type '{type}'. Valid emotions are: {string.Join(", ", Enum.GetNames<EmotionType>())}.");
        }

        ValidatePercentage(percentage);

        Id = Guid.NewGuid();
        HumanId = humanId;
        Type = type;
        Percentage = Math.Round(percentage, 2);
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdatePercentage(double newPercentage)
    {
        ValidatePercentage(newPercentage);
        Percentage = Math.Round(newPercentage, 2);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static void ValidatePercentage(double percentage)
    {
        if (percentage < 0.0 || percentage > 100.0)
        {
            throw new DomainException($"Emotion percentage must be between 0.0 and 100.0. Received: {percentage}.");
        }
    }
}

