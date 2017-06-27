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
        public void Run_ThrowsExceptionIfPluginOptionsIsNotAnMSBuildPluginOptionsInstance()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.PluginOptions).Returns((IPluginOptions)null);

            Mock<IMSBuildService> mockMSBuildService = _mockRepository.Create<IMSBuildService>();

            MSBuildPlugin plugin = new MSBuildPlugin(mockMSBuildService.Object, null, null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => plugin.Run(null, mockStepContext.Object));
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_DoesNotRunMSBuildOnDryRun()
        {
            // Arrange
            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.PluginOptions).Returns(new MSBuildPluginOptions());

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<PipelineOptions> mockPipelineOptions = Mock.Get(mockPipelineContext.Object.PipelineOptions);
            mockPipelineOptions.Setup(s => s.DryRun).Returns(true);

            Mock<IMSBuildService> mockMSBuildService = _mockRepository.Create<IMSBuildService>();
            Mock<ILoggingService<MSBuildPlugin>> mockLoggingService = _mockRepository.Create<ILoggingService<MSBuildPlugin>>();
            Mock<IDirectoryService> mockDirectoryService = _mockRepository.Create<IDirectoryService>();

            MSBuildPlugin plugin = new MSBuildPlugin(mockMSBuildService.Object, mockLoggingService.Object, mockDirectoryService.Object);

            // Act
            plugin.Run(mockPipelineContext.Object, mockStepContext.Object);

            // Assert
            _mockRepository.VerifyAll();
            mockMSBuildService.
                Verify(m => m.Build(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Run_RunsMSBuildServiceBuildWithSpecifiedArguments()
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

            Mock<ILoggingService<MSBuildPlugin>> mockLoggingService = _mockRepository.Create<ILoggingService<MSBuildPlugin>>();
            Mock<IDirectoryService> mockDirectoryService = _mockRepository.Create<IDirectoryService>();

            MSBuildPlugin plugin = new MSBuildPlugin(mockMSBuildService.Object, mockLoggingService.Object, mockDirectoryService.Object);

            // Act
            plugin.Run(mockPipelineContext.Object, mockStepContext.Object);

            // Assert
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_LogsCorrectMessageWhenProjOrSlnFileIsNullOrAnEmptyString()
        {
            // Arrange
            string testSwitches = "testSwitches";
            string testDirectory = "testDirectory";

            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.PluginOptions).Returns(new MSBuildPluginOptions
            {
                ProjOrSlnFile = null,
                Switches = testSwitches
            });

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<IMSBuildService> mockMSBuildService = _mockRepository.Create<IMSBuildService>();

            Mock<IDirectoryService> mockDirectoryService = _mockRepository.Create<IDirectoryService>();
            mockDirectoryService.Setup(d => d.GetCurrentDirectory()).Returns(testDirectory);

            Mock<ILoggingService<MSBuildPlugin>> mockLoggingService = _mockRepository.Create<ILoggingService<MSBuildPlugin>>();
            mockLoggingService.Setup(l => l.LogInformation(Strings.Log_RanMSBuildInDir, testDirectory, testSwitches));

            MSBuildPlugin plugin = new MSBuildPlugin(mockMSBuildService.Object, mockLoggingService.Object, mockDirectoryService.Object);

            // Act
            plugin.Run(mockPipelineContext.Object, mockStepContext.Object);

            // Assert
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Run_LogsCorrectMessageWhenProjOrSlnFileIsNotNullOrAnEmptyString()
        {
            // Arrange
            string testSwitches = "testSwitches";
            string testFile = "testFile";

            Mock<IStepContext> mockStepContext = _mockRepository.Create<IStepContext>();
            mockStepContext.Setup(s => s.PluginOptions).Returns(new MSBuildPluginOptions
            {
                ProjOrSlnFile = testFile,
                Switches = testSwitches
            });

            Mock<IPipelineContext> mockPipelineContext = _mockRepository.Create<IPipelineContext>();
            Mock<IMSBuildService> mockMSBuildService = _mockRepository.Create<IMSBuildService>();

            Mock<ILoggingService<MSBuildPlugin>> mockLoggingService = _mockRepository.Create<ILoggingService<MSBuildPlugin>>();
            mockLoggingService.Setup(l => l.LogInformation(Strings.Log_RanMSBuildOnFile, testFile, testSwitches));

            MSBuildPlugin plugin = new MSBuildPlugin(mockMSBuildService.Object, mockLoggingService.Object, null);

            // Act
            plugin.Run(mockPipelineContext.Object, mockStepContext.Object);

            // Assert
            _mockRepository.VerifyAll();
        }
    }
}
