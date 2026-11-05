using HumanEmotionApi.Domain.Exceptions;
using Xunit;

namespace HumanEmotionApi.Tests.Domain;

public class ExceptionsTests
{
    [Fact]
    public void DomainException_Constructors_ShouldSetProperties()
    {
        var ex1 = new DomainException("Test error");
        Assert.Equal("Test error", ex1.Message);

        var inner = new InvalidOperationException("Inner");
        var ex2 = new DomainException("Outer", inner);
        Assert.Equal("Outer", ex2.Message);
        Assert.Same(inner, ex2.InnerException);
    }

    [Fact]
    public void NotFoundException_Constructors_ShouldSetProperties()
    {
        var ex1 = new NotFoundException("Not found message");
        Assert.Equal("Not found message", ex1.Message);

        var id = Guid.NewGuid();
        var ex2 = new NotFoundException("Human", id);
        Assert.Contains("Human", ex2.Message);
        Assert.Contains(id.ToString(), ex2.Message);
    }
}

