using JeremyTCD.PipelinesCE.Tools;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.PipelinesCE.Plugin.Nuget.Tests.UnitTests
{
    public class NugetChangelogAdapterUnitTests
    {
        private MockRepository _mockRepository { get; }

        public NugetChangelogAdapterUnitTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void Run_ThrowsExceptionIfPluginOptionsIsNotANugetChangelogAdapterOptionsInstance()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.PluginOptions).Returns((IPluginOptions)null);

            Mock<INugetClient> mockNugetClient = _mockRepository.Create<INugetClient>();

            NugetChangelogAdapter adapter = new NugetChangelogAdapter(mockNugetClient.Object);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => adapter.Run(null, mockStepContext.Object));
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_ThrowsExceptionIfSharedDataDoesNotContainChangelog()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.PluginOptions).Returns(new NugetChangelogAdapterOptions());

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<IDictionary<string, object>> mockSharedData = Mock.Get(mockPipelineContext.Object.SharedData);
            object outValue = null;
            mockSharedData.Setup(s => s.TryGetValue(nameof(Changelog), out outValue));

            Mock<INugetClient> mockNugetClient = _mockRepository.Create<INugetClient>();

            NugetChangelogAdapter adapter = new NugetChangelogAdapter(mockNugetClient.Object);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => adapter.Run(mockPipelineContext.Object, mockStepContext.Object));
            _mockRepository.VerifyAll();
        }
    }
}
