using FluentAssertions;
using System;
using Xunit;

namespace Devit.Commanding.Tests;

public class UpdateTests
{
    [Fact]
    public void TestImplicitConversionFromString()
    {
        Update<string> value = "123";
        value.Operation.Should().Be(UpdateOperation.Set);
        value.GetValue().Should().Be("123");

        ((string)value).Should().Be("123");
        ((string?)value).Should().Be("123");
    }

    [Fact]
    public void TestNotProvided()
    {
        Update<int> value = Update.NotProvided;

        value.Operation.Should().Be(UpdateOperation.NotProvided);

        var action1 = () => value.GetValue();
        action1.Should().Throw<InvalidOperationException>();

        var action2 = () => (int)value;
        action2.Should().Throw<InvalidOperationException>();
    }
}