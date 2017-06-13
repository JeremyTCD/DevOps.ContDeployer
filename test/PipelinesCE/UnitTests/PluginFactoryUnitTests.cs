using System;
using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using Moq;
using Xunit;
using StructureMap;

namespace JeremyTCD.PipelinesCE.Tests.UnitTests
{
    public class PluginFactoryUnitTests
    {
        private MockRepository _mockRepository { get; }

        public PluginFactoryUnitTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void CreatePlugin_ThrowsExceptionIfPluginTypeHasNoCorrespondingStructureMapProfile()
        {
            // Arrange
            string pluginName = nameof(DummyPlugin);
            Type pluginType = typeof(DummyPlugin);
            DummyPlugin stubPlugin = new DummyPlugin();

            Mock<ILoggingService<PluginFactory>> mockLoggingService = _mockRepository.Create<ILoggingService<PluginFactory>>();
            mockLoggingService.Setup(l => l.LogDebug(string.Format(Strings.Exception_NoContainerForPluginType, pluginName)));

            Mock<IContainer> mockContainer = _mockRepository.Create<IContainer>();
            mockContainer.Setup(c => c.GetProfile(pluginName)).Returns((IContainer)null);

            PluginFactory pluginFactory = new PluginFactory(mockLoggingService.Object, mockContainer.Object);

            // Act and assert
            Exception exception = Assert.Throws<Exception>(() => pluginFactory.CreatePlugin(pluginType));
            Assert.Equal(string.Format(Strings.Exception_NoContainerForPluginType, pluginName), exception.Message);
        }

        [Fact]
        public void CreatePlugin_ThrowsExceptionIfStructureMapContainerHasNoServiceForTypePluginType()
        {
            // Arrange
            string pluginName = nameof(DummyPlugin);
            Type pluginType = typeof(DummyPlugin);
            DummyPlugin stubPlugin = new DummyPlugin();

            Mock<ILoggingService<PluginFactory>> mockLoggingService = _mockRepository.Create<ILoggingService<PluginFactory>>();
            mockLoggingService.Setup(l => l.LogDebug(string.Format(Strings.Exception_NoContainerForPluginType, pluginName)));

            Mock<IContainer> mockContainer = _mockRepository.Create<IContainer>();
            mockContainer.Setup(c => c.GetProfile(pluginName)).Returns(mockContainer.Object);
            mockContainer.Setup(c => c.GetInstance(pluginType)).Returns(null);

            PluginFactory pluginFactory = new PluginFactory(mockLoggingService.Object, mockContainer.Object);

            // Act and assert
            Exception exception = Assert.Throws<Exception>(() => pluginFactory.CreatePlugin(pluginType));
            Assert.Equal(string.Format(Strings.Exception_NoServiceForPluginType, pluginName), exception.Message);
        }

        [Fact]
        public void CreatePlugin_ReturnsIPluginInstanceOfTypePluginType()
        {
            // Arrange
            string pluginName = nameof(DummyPlugin);
            Type pluginType = typeof(DummyPlugin);
            DummyPlugin stubPlugin = new DummyPlugin();

            Mock<ILoggingService<PluginFactory>> mockLoggingService = _mockRepository.Create<ILoggingService<PluginFactory>>();
            mockLoggingService.Setup(l => l.LogDebug(string.Format(Strings.Exception_NoContainerForPluginType, pluginName)));

            Mock<IContainer> mockContainer = _mockRepository.Create<IContainer>();
            mockContainer.Setup(c => c.GetProfile(pluginName)).Returns(mockContainer.Object);
            mockContainer.Setup(c => c.GetInstance(pluginType)).Returns(stubPlugin);

            PluginFactory pluginFactory = new PluginFactory(mockLoggingService.Object, mockContainer.Object);

            // Act
            IPlugin result = pluginFactory.CreatePlugin(pluginType);

            // Assert
            Assert.Equal(stubPlugin, result);
        }

        private class DummyPlugin : IPlugin
        {
            public void Run(IPipelineContext pipelineContext, IStepContext stepContext)
            {
                throw new NotImplementedException();
            }
        }
    }
}
