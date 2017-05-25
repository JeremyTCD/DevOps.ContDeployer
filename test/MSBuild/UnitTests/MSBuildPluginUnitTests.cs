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

            Mock<IMSBuildClient> mockMSBuildClient = _mockRepository.Create<IMSBuildClient>();

            MSBuildPlugin plugin = new MSBuildPlugin(mockMSBuildClient.Object);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => plugin.Run(null, mockStepContext.Object));
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_CallsMSBuildClientBuildWithSpecifiedArguments()
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

            Mock<IMSBuildClient> mockMSBuildClient = _mockRepository.Create<IMSBuildClient>();
            mockMSBuildClient.Setup(m => m.Build(testFile, testSwitches));

            MSBuildPlugin plugin = new MSBuildPlugin(mockMSBuildClient.Object);

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

            Mock<IMSBuildClient> mockMSBuildClient = _mockRepository.Create<IMSBuildClient>();

            MSBuildPlugin plugin = new MSBuildPlugin(mockMSBuildClient.Object);

            // Act
            plugin.Run(mockPipelineContext.Object, mockStepContext.Object);

            // Assert
            _mockRepository.VerifyAll();
            mockMSBuildClient.
                Verify(m => m.Build(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
