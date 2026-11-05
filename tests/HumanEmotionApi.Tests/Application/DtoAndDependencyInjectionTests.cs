using HumanEmotionApi.Application;
using HumanEmotionApi.Application.DTOs;
using HumanEmotionApi.Domain.Entities;
using HumanEmotionApi.Domain.Enums;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HumanEmotionApi.Tests.Application;

public class DtoAndDependencyInjectionTests
{
    [Fact]
    public void EmotionDto_FromEntity_ShouldMapProperties()
    {
        var humanId = Guid.NewGuid();
        var emotion = new Emotion(humanId, EmotionType.Joy, 88.5);

        var dto = EmotionDto.FromEntity(emotion);

        Assert.Equal(emotion.Id, dto.Id);
        Assert.Equal(humanId, dto.HumanId);
        Assert.Equal("Joy", dto.Type);
        Assert.Equal(88.5, dto.Percentage);
        Assert.Equal(emotion.CreatedAtUtc, dto.CreatedAtUtc);
        Assert.Null(dto.UpdatedAtUtc);
    }

    [Fact]
    public void HumanDto_FromEntity_ShouldMapPropertiesAndShowColor()
    {
        var human = new Human("Bob");
        human.AddEmotion(EmotionType.Joy, 100.0);

        var dto = HumanDto.FromEntity(human);

        Assert.Equal(human.Id, dto.Id);
        Assert.Equal("Bob", dto.Name);
        Assert.Equal(human.ShowColor, dto.ShowColor);
        Assert.Equal(human.EmotionScorePercentage, dto.EmotionScorePercentage);
        Assert.Single(dto.Emotions);
        Assert.Equal("Joy", dto.Emotions[0].Type);
    }

    [Fact]
    public void AddApplication_ShouldRegisterMediatRServices()
    {
        var services = new ServiceCollection();
        services.AddApplication();

        var provider = services.BuildServiceProvider();
        var mediator = provider.GetService<IMediator>();

        Assert.NotNull(mediator);
    }
}

