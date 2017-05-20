using JeremyTCD.ContDeployer.Plugin.Changelog;
using JeremyTCD.ContDeployer.PluginTools;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.Nuget.Tests.UnitTests
{
    public class NugetChangelogAdapterUnitTests
    {
        private MockRepository _mockRepository { get; }

        public NugetChangelogAdapterUnitTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void Constructor_ThrowsExceptionIfOptionsIsNullOrNotNugetChangelogAdapterOptionsInstance()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.PluginOptions).Returns((IPluginOptions)null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new NugetChangelogAdapter(null, mockStepContext.Object, null));
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Constructor_ThrowsExceptionIfSharedDataDoesNotContainChangelog()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.PluginOptions).Returns(new NugetChangelogAdapterOptions());

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<IDictionary<string, object>> mockSharedData = Mock.Get(mockPipelineContext.Object.SharedData);
            object outValue = null;
            mockSharedData.Setup(s => s.TryGetValue(nameof(Changelog), out outValue));

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new NugetChangelogAdapter(mockPipelineContext.Object, 
                mockStepContext.Object, null));
            _mockRepository.VerifyAll();
        }
    }
}
