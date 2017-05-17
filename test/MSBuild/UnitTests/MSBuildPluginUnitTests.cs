using JeremyTCD.ContDeployer.PluginTools;
using Moq;
using System;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.MSBuild.Tests.UnitTests
{
    public class MSBuildPluginUnitTests
    {
        private MockRepository _mockRepository { get; }

        public MSBuildPluginUnitTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void Constructor_ThrowsExceptionIfOptionsIsNullOrNotAGitOptionsInstance()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.Options).Returns((IPluginOptions)null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => new MSBuildPlugin(null, null, mockStepContext.Object));
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_CallsMSBuildClientBuildWithSpecifiedArguments()
        {
            // Arrange
            string testFile = "testFile";
            string testSwitches = "testSwitches";

            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.Options).Returns(new MSBuildPluginOptions
            {
                ProjOrSlnFile = testFile,
                Switches = testSwitches
            });

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<SharedOptions> mockSharedOptions = Mock.Get(mockPipelineContext.Object.SharedOptions);
            mockSharedOptions.Setup(s => s.DryRun).Returns(false);

            Mock<IMSBuildClient> mockMSBuildClient = _mockRepository.Create<IMSBuildClient>();
            mockMSBuildClient.Setup(m => m.Build(testFile, testSwitches));

            MSBuildPlugin plugin = new MSBuildPlugin(mockMSBuildClient.Object, mockPipelineContext.Object,
                mockStepContext.Object);

            // Act
            plugin.Run();

            // Assert
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_DoesNotAddATagOnDryRun()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.Options).Returns(new MSBuildPluginOptions());

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<SharedOptions> mockSharedOptions = Mock.Get(mockPipelineContext.Object.SharedOptions);
            mockSharedOptions.Setup(s => s.DryRun).Returns(true);

            Mock<IMSBuildClient> mockMSBuildClient = _mockRepository.Create<IMSBuildClient>();

            MSBuildPlugin plugin = new MSBuildPlugin(mockMSBuildClient.Object, mockPipelineContext.Object,
                mockStepContext.Object);

            // Act
            plugin.Run();

            // Assert
            _mockRepository.VerifyAll();
            mockMSBuildClient.
                Verify(m => m.Build(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
