using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KafkaFlow.UnitTests.ConfigurationBuilders;

[TestClass]
public class TypedHandlerConfigurationBuilderTests
{
    private readonly Mock<IDependencyConfigurator> _dependencyConfiguratorMock;
    private readonly TypedHandlerConfigurationBuilder _builder;

    public TypedHandlerConfigurationBuilderTests()
    {
        _dependencyConfiguratorMock = new Mock<IDependencyConfigurator>();
        _builder = new TypedHandlerConfigurationBuilder(_dependencyConfiguratorMock.Object);
    }

    [TestMethod]
    public void Build_ShouldReturnConfigurationWithHandlers()
    {
        // Arrange
        _builder.AddHandler<SampleHandler>();

        // Act
        var configuration = _builder.Build();

        // Assert
        configuration.Should().NotBeNull();
        configuration.HandlerMapping.GetHandlersTypes(typeof(SampleMessage))
            .Should().Contain(typeof(SampleHandler));
    }

    [TestMethod]
    public void Build_ShouldRegisterHandlersInDependencyConfigurator()
    {
        // Arrange
        _builder.AddHandler<SampleHandler>();

        // Act
        _builder.Build();

        // Assert
        _dependencyConfiguratorMock.Verify(
            x => x.Add(typeof(SampleHandler), typeof(SampleHandler), InstanceLifetime.Singleton),
            Times.Once);
    }

    [TestMethod]
    public void Build_ShouldNotRegisterAlreadyRegisteredHandlers()
    {
        // Arrange
        _dependencyConfiguratorMock
            .Setup(x => x.IsRegistered(typeof(SampleHandler)))
            .Returns(true);

        _builder.AddHandler<SampleHandler>();

        // Act
        _builder.Build();

        // Assert
        _dependencyConfiguratorMock.Verify(
            x => x.Add(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<InstanceLifetime>()),
            Times.Never);
    }

    [TestMethod]
    public void Build_ShouldSetOnNoHandlerFound()
    {
        // Arrange
        Action<IMessageContext> onNoHandlerFound = _ => { };
        _builder.WhenNoHandlerFound(onNoHandlerFound);

        // Act
        var configuration = _builder.Build();

        // Assert
        configuration.OnNoHandlerFound.Should().Be(onNoHandlerFound);
    }

    private class SampleMessage { }

    private class SampleHandler : IMessageHandler<SampleMessage>
    {
        public Task Handle(IMessageContext context, SampleMessage message) => Task.CompletedTask;
    }
}