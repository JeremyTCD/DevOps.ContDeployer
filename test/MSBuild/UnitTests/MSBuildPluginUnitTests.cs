using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using Moq;
using System;
using Xunit;

namespace JeremyTCD.PipelinesCE.Plugin.MSBuild.Tests.UnitTests
{
    public class MSBuildPluginUnitTests
    {
        private MockRepository _mockRepository { get; }

        public MSBuildPluginUnitTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void Run_ThrowsExceptionIfPluginOptionsIsNotAGitOptionsInstance()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.PluginOptions).Returns((IPluginOptions)null);

            Mock<IMSBuildService> mockMSBuildService = _mockRepository.Create<IMSBuildService>();

            MSBuildPlugin plugin = new MSBuildPlugin(mockMSBuildService.Object);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => plugin.Run(null, mockStepContext.Object));
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_CallsMSBuildServiceBuildWithSpecifiedArguments()
        {
            // Arrange
            string testFile = "testFile";
            string testSwitches = "testSwitches";

            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.PluginOptions).Returns(new MSBuildPluginOptions
            {
                ProjOrSlnFile = testFile,
                Switches = testSwitches
            });

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<PipelineOptions> mockPipelineOptions = Mock.Get(mockPipelineContext.Object.PipelineOptions);
            mockPipelineOptions.Setup(s => s.DryRun).Returns(false);

            Mock<IMSBuildService> mockMSBuildService = _mockRepository.Create<IMSBuildService>();
            mockMSBuildService.Setup(m => m.Build(testFile, testSwitches));

            MSBuildPlugin plugin = new MSBuildPlugin(mockMSBuildService.Object);

            // Act
            plugin.Run(mockPipelineContext.Object, mockStepContext.Object);

            // Assert
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_DoesNotAddATagOnDryRun()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.PluginOptions).Returns(new MSBuildPluginOptions());

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<PipelineOptions> mockPipelineOptions = Mock.Get(mockPipelineContext.Object.PipelineOptions);
            mockPipelineOptions.Setup(s => s.DryRun).Returns(true);

            Mock<IMSBuildService> mockMSBuildService = _mockRepository.Create<IMSBuildService>();

            MSBuildPlugin plugin = new MSBuildPlugin(mockMSBuildService.Object);

            // Act
            plugin.Run(mockPipelineContext.Object, mockStepContext.Object);

            // Assert
            _mockRepository.VerifyAll();
            mockMSBuildService.
                Verify(m => m.Build(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
