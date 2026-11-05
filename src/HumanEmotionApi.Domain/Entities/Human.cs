using HumanEmotionApi.Domain.Enums;
using HumanEmotionApi.Domain.Exceptions;

namespace HumanEmotionApi.Domain.Entities;

public class Human
{
    private readonly List<Emotion> _emotions = new();

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public IReadOnlyCollection<Emotion> Emotions => _emotions.AsReadOnly();
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Hex color code representing emotional state from pure white (#FFFFFF) to deep green (#008000)
    /// based on the basic 5 emotions added and their respective percentages.
    /// </summary>
    public string ShowColor => CalculateShowColor();

    /// <summary>
    /// Overall emotion fullness score from 0.0% to 100.0%.
    /// </summary>
    public double EmotionScorePercentage => CalculateEmotionScore() * 100.0;

    // EF Core constructor
    protected Human() { }

    public Human(string name)
    {
        ValidateName(name);

        Id = Guid.NewGuid();
        Name = name.Trim();
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateName(string newName)
    {
        ValidateName(newName);
        Name = newName.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public Emotion AddEmotion(EmotionType type, double percentage)
    {
        if (_emotions.Any(e => e.Type == type))
        {
            throw new DomainException($"Emotion '{type}' is already present on this human. Use update to modify its percentage.");
        }

        var emotion = new Emotion(Id, type, percentage);
        _emotions.Add(emotion);
        UpdatedAtUtc = DateTime.UtcNow;
        return emotion;
    }

    public Emotion UpdateEmotion(EmotionType type, double newPercentage)
    {
        var existing = _emotions.FirstOrDefault(e => e.Type == type);
        if (existing == null)
        {
            throw new NotFoundException($"Emotion '{type}' was not found on human '{Id}'.");
        }

        existing.UpdatePercentage(newPercentage);
        UpdatedAtUtc = DateTime.UtcNow;
        return existing;
    }

    public bool RemoveEmotion(EmotionType type)
    {
        var existing = _emotions.FirstOrDefault(e => e.Type == type);
        if (existing == null)
        {
            return false;
        }

        _emotions.Remove(existing);
        UpdatedAtUtc = DateTime.UtcNow;
        return true;
    }

    public bool HasEmotion(EmotionType type) => _emotions.Any(e => e.Type == type);

    private double CalculateEmotionScore()
    {
        if (_emotions.Count == 0)
        {
            return 0.0;
        }

        // 5 basic emotions, each can reach up to 100.0%, max sum = 500.0
        const double maxPossibleTotal = 500.0;
        double currentTotal = _emotions.Sum(e => e.Percentage);
        double normalized = currentTotal / maxPossibleTotal;

        return Math.Clamp(normalized, 0.0, 1.0);
    }

    private string CalculateShowColor()
    {
        double score = CalculateEmotionScore();

        // White: RGB(255, 255, 255)
        // Deep Green: RGB(0, 128, 0)
        // Linear interpolation from White (score=0.0) to Deep Green (score=1.0)
        const int startR = 255, startG = 255, startB = 255;
        const int targetR = 0, targetG = 128, targetB = 0;

        int r = (int)Math.Round(startR - ((startR - targetR) * score));
        int g = (int)Math.Round(startG - ((startG - targetG) * score));
        int b = (int)Math.Round(startB - ((startB - targetB) * score));

        r = Math.Clamp(r, 0, 255);
        g = Math.Clamp(g, 0, 255);
        b = Math.Clamp(b, 0, 255);

        return $"#{r:X2}{g:X2}{b:X2}";
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Human name cannot be null, empty, or consist only of whitespace.");
        }

        if (name.Trim().Length > 100)
        {
            throw new DomainException("Human name cannot exceed 100 characters.");
        }
    }
}

