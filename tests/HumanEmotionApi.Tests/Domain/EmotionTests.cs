using HumanEmotionApi.Domain.Entities;
using HumanEmotionApi.Domain.Enums;
using HumanEmotionApi.Domain.Exceptions;
using Xunit;

namespace HumanEmotionApi.Tests.Domain;

public class EmotionTests
{
    [Fact]
    public void Constructor_WithValidArguments_ShouldInitializeCorrectly()
    {
        // Arrange
        var humanId = Guid.NewGuid();

        // Act
        var emotion = new Emotion(humanId, EmotionType.Joy, 75.5);

        // Assert
        Assert.NotEqual(Guid.Empty, emotion.Id);
        Assert.Equal(humanId, emotion.HumanId);
        Assert.Equal(EmotionType.Joy, emotion.Type);
        Assert.Equal(75.5, emotion.Percentage);
        Assert.True(emotion.CreatedAtUtc <= DateTime.UtcNow);
        Assert.Null(emotion.UpdatedAtUtc);
    }

    [Fact]
    public void Constructor_WithEmptyHumanId_ShouldThrowDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new Emotion(Guid.Empty, EmotionType.Joy, 50.0));
        Assert.Contains("HumanId cannot be empty", ex.Message);
    }

    [Fact]
    public void Constructor_WithInvalidEmotionType_ShouldThrowDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new Emotion(Guid.NewGuid(), (EmotionType)999, 50.0));
        Assert.Contains("Invalid emotion type", ex.Message);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(-50.0)]
    [InlineData(100.1)]
    [InlineData(200.0)]
    public void Constructor_WithOutOfRangePercentage_ShouldThrowDomainException(double invalidPercentage)
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new Emotion(Guid.NewGuid(), EmotionType.Sadness, invalidPercentage));
        Assert.Contains("between 0.0 and 100.0", ex.Message);
    }

    [Fact]
    public void UpdatePercentage_WithValidValue_ShouldUpdateAndSetTimestamp()
    {
        // Arrange
        var emotion = new Emotion(Guid.NewGuid(), EmotionType.Anger, 25.0);

        // Act
        emotion.UpdatePercentage(80.0);

        // Assert
        Assert.Equal(80.0, emotion.Percentage);
        Assert.NotNull(emotion.UpdatedAtUtc);
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(101.0)]
    public void UpdatePercentage_WithOutOfRangeValue_ShouldThrowDomainException(double invalidPercentage)
    {
        // Arrange
        var emotion = new Emotion(Guid.NewGuid(), EmotionType.Anger, 25.0);

        // Act & Assert
        Assert.Throws<DomainException>(() => emotion.UpdatePercentage(invalidPercentage));
    }
}

