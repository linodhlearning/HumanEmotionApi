using HumanEmotionApi.Domain.Entities;
using HumanEmotionApi.Domain.Enums;
using HumanEmotionApi.Domain.Exceptions;
using Xunit;

namespace HumanEmotionApi.Tests.Domain;

public class HumanTests
{
    [Fact]
    public void Constructor_WithValidName_ShouldInitializeCorrectly()
    {
        // Act
        var human = new Human("Alice");

        // Assert
        Assert.NotEqual(Guid.Empty, human.Id);
        Assert.Equal("Alice", human.Name);
        Assert.Empty(human.Emotions);
        Assert.Equal("#FFFFFF", human.ShowColor);
        Assert.Equal(0.0, human.EmotionScorePercentage);
        Assert.True(human.CreatedAtUtc <= DateTime.UtcNow);
        Assert.Null(human.UpdatedAtUtc);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_ShouldThrowDomainException(string? invalidName)
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new Human(invalidName!));
        Assert.Contains("cannot be null, empty", ex.Message);
    }

    [Fact]
    public void Constructor_WithNameExceeding100Chars_ShouldThrowDomainException()
    {
        // Arrange
        string longName = new('A', 101);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new Human(longName));
        Assert.Contains("cannot exceed 100 characters", ex.Message);
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldUpdateNameAndTimestamp()
    {
        // Arrange
        var human = new Human("Alice");

        // Act
        human.UpdateName("Bob");

        // Assert
        Assert.Equal("Bob", human.Name);
        Assert.NotNull(human.UpdatedAtUtc);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void UpdateName_WithInvalidName_ShouldThrowDomainException(string? invalidName)
    {
        // Arrange
        var human = new Human("Alice");

        // Act & Assert
        Assert.Throws<DomainException>(() => human.UpdateName(invalidName!));
    }

    [Fact]
    public void AddEmotion_WithValidEmotion_ShouldAddAndRecalculateShowColor()
    {
        // Arrange
        var human = new Human("Alice");

        // Act
        var emotion = human.AddEmotion(EmotionType.Joy, 100.0);

        // Assert
        Assert.Single(human.Emotions);
        Assert.Equal(EmotionType.Joy, emotion.Type);
        Assert.Equal(100.0, emotion.Percentage);
        Assert.True(human.HasEmotion(EmotionType.Joy));
        Assert.NotNull(human.UpdatedAtUtc);

        // Score = 100 / 500 = 0.20
        // R = 255 - 51 = 204 (CC)
        // G = 255 - 25.4 = 230 (E6)
        // B = 255 - 51 = 204 (CC)
        Assert.Equal("#CCE6CC", human.ShowColor);
        Assert.Equal(20.0, human.EmotionScorePercentage);
    }

    [Fact]
    public void AddEmotion_DuplicateEmotion_ShouldThrowDomainException()
    {
        // Arrange
        var human = new Human("Alice");
        human.AddEmotion(EmotionType.Joy, 50.0);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => human.AddEmotion(EmotionType.Joy, 70.0));
        Assert.Contains("already present", ex.Message);
    }

    [Fact]
    public void ShowColor_WithAllFiveEmotionsAt100Percent_ShouldBePureDeepGreen()
    {
        // Arrange
        var human = new Human("Alice");

        // Act - Add all 5 basic emotions at 100%
        human.AddEmotion(EmotionType.Joy, 100.0);
        human.AddEmotion(EmotionType.Sadness, 100.0);
        human.AddEmotion(EmotionType.Anger, 100.0);
        human.AddEmotion(EmotionType.Fear, 100.0);
        human.AddEmotion(EmotionType.Disgust, 100.0);

        // Assert
        // Score = 500 / 500 = 1.0
        // Target green: RGB(0, 128, 0) -> #008000
        Assert.Equal("#008000", human.ShowColor);
        Assert.Equal(100.0, human.EmotionScorePercentage);
    }

    [Fact]
    public void ShowColor_WithAllFiveEmotionsAt50Percent_ShouldBeHalfwayGreen()
    {
        // Arrange
        var human = new Human("Alice");

        // Act - Add all 5 emotions at 50%
        human.AddEmotion(EmotionType.Joy, 50.0);
        human.AddEmotion(EmotionType.Sadness, 50.0);
        human.AddEmotion(EmotionType.Anger, 50.0);
        human.AddEmotion(EmotionType.Fear, 50.0);
        human.AddEmotion(EmotionType.Disgust, 50.0);

        // Assert
        // Score = 250 / 500 = 0.5
        // R = 255 - 127.5 = 128 (80)
        // G = 255 - 63.5 = 192 (C0)
        // B = 255 - 127.5 = 128 (80)
        Assert.Equal("#80C080", human.ShowColor);
        Assert.Equal(50.0, human.EmotionScorePercentage);
    }

    [Fact]
    public void UpdateEmotion_WithValidPercentage_ShouldUpdateAndRecalculateColor()
    {
        // Arrange
        var human = new Human("Alice");
        human.AddEmotion(EmotionType.Joy, 10.0);

        // Act
        var updated = human.UpdateEmotion(EmotionType.Joy, 100.0);

        // Assert
        Assert.Equal(100.0, updated.Percentage);
        Assert.Equal("#CCE6CC", human.ShowColor);
    }

    [Fact]
    public void UpdateEmotion_ForNonExistentEmotion_ShouldThrowNotFoundException()
    {
        // Arrange
        var human = new Human("Alice");

        // Act & Assert
        Assert.Throws<NotFoundException>(() => human.UpdateEmotion(EmotionType.Joy, 50.0));
    }

    [Fact]
    public void RemoveEmotion_ForExistingEmotion_ShouldReturnTrueAndRevertColor()
    {
        // Arrange
        var human = new Human("Alice");
        human.AddEmotion(EmotionType.Joy, 100.0);
        Assert.Equal("#CCE6CC", human.ShowColor);

        // Act
        bool removed = human.RemoveEmotion(EmotionType.Joy);

        // Assert
        Assert.True(removed);
        Assert.Empty(human.Emotions);
        Assert.False(human.HasEmotion(EmotionType.Joy));
        Assert.Equal("#FFFFFF", human.ShowColor);
        Assert.Equal(0.0, human.EmotionScorePercentage);
    }

    [Fact]
    public void RemoveEmotion_ForNonExistentEmotion_ShouldReturnFalse()
    {
        // Arrange
        var human = new Human("Alice");

        // Act
        bool removed = human.RemoveEmotion(EmotionType.Joy);

        // Assert
        Assert.False(removed);
    }
}

