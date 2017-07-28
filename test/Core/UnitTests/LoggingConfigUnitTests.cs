using JeremyTCD.DotNetCore.Utils;
using Moq;
using NLog;
using NLog.Config;
using NLog.Targets;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace JeremyTCD.PipelinesCE.Core.Tests.UnitTests
{
    public class LoggingConfigUnitTests
    {
        private MockRepository _mockRepository { get; }

        public LoggingConfigUnitTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default) { DefaultValue = DefaultValue.Mock };
        }

        [Theory]
        [MemberData(nameof(SetsLogLevelAccordingToDebugAndVerboseOptionsData))]
        public void CreateLoggingConfiguration_SetsLogLevelAccordingToDebugAndVerboseOptions(bool debug, bool verbose, LogLevel expectedLogLevel)
        {
            // Arrange
            string testLogFile = "testLogFile";
            PipelinesCEOptions stubPipelinesCEOptions = new PipelinesCEOptions
            {
                Debug = debug,
                Verbose = verbose,
                LogFile = testLogFile
            };

            Mock<IPathService> mockPathService = _mockRepository.Create<IPathService>();
            mockPathService.Setup(p => p.IsPathRooted(testLogFile)).Returns(true);

            LoggingConfig loggingConfig = new LoggingConfig(mockPathService.Object, null);

            // Act
            LoggingConfiguration result = loggingConfig.CreateLoggingConfiguration(stubPipelinesCEOptions);

            // Assert
            _mockRepository.VerifyAll();
            Assert.True(result.LoggingRules.All(r => r.Levels.First() == expectedLogLevel));
        }

        public static IEnumerable<object[]> SetsLogLevelAccordingToDebugAndVerboseOptionsData()
        {
            yield return new object[] { false, false, LogLevel.Info };
            yield return new object[] { false, true, LogLevel.Debug };
            yield return new object[] { true, false, LogLevel.Debug };
            yield return new object[] { true, true, LogLevel.Debug };
        }

        [Fact]
        public void CreateLoggingConfiguration_RootsLogFileIfItIsNotRooted()
        {
            // Arrange
            string testLogFile = "testLogFile";
            string testProjectFile = "testProjectFile";
            PipelinesCEOptions stubPipelinesCEOptions = new PipelinesCEOptions
            {
                LogFile = testLogFile,
                ProjectFile = testProjectFile
            };
            DirectoryInfo stubDirectoryInfo = new DirectoryInfo("C:/test");
            string testProjectDir = stubDirectoryInfo.FullName;

            Mock<IPathService> mockPathService = _mockRepository.Create<IPathService>();
            mockPathService.Setup(p => p.IsPathRooted(testLogFile)).Returns(false);
            mockPathService.Setup(p => p.Combine(testProjectDir, testLogFile)).Returns(testLogFile);

            Mock<IDirectoryService> mockDirectoryService = _mockRepository.Create<IDirectoryService>();
            mockDirectoryService.Setup(d => d.GetParent(testProjectFile)).Returns(stubDirectoryInfo);

            LoggingConfig loggingConfig = new LoggingConfig(mockPathService.Object, mockDirectoryService.Object);

            // Act
            LoggingConfiguration result = loggingConfig.CreateLoggingConfiguration(stubPipelinesCEOptions);

            // Assert
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void CreateLoggingConfiguration_CreatesLoggingConfig()
        {
            // TODO creates console target
        }

        // TODO update function so filetarget isn't created unless logfile is specified, write test 

        [Theory]
        [MemberData(nameof(CreatesDebuggerTargetIfDebugOrVerboseOptionsAreTrueData))]
        public void CreateLoggingConfiguration_CreatesDebuggerTargetIfDebugOrVerboseOptionsAreTrue(bool debug, bool verbose, bool expected)
        {
            // Arrange
            string testLogFile = "testLogFile";
            PipelinesCEOptions stubPipelinesCEOptions = new PipelinesCEOptions
            {
                Debug = debug,
                Verbose = verbose,
                LogFile = testLogFile
            };

            Mock<IPathService> mockPathService = _mockRepository.Create<IPathService>();
            mockPathService.Setup(p => p.IsPathRooted(testLogFile)).Returns(true);

            LoggingConfig loggingConfig = new LoggingConfig(mockPathService.Object, null);

            // Act
            LoggingConfiguration result = loggingConfig.CreateLoggingConfiguration(stubPipelinesCEOptions);

            // Assert
            _mockRepository.VerifyAll();
            Assert.Equal(expected, result.ConfiguredNamedTargets.Any(r => r.GetType() == typeof(DebuggerTarget)));
        }

        public static IEnumerable<object[]> CreatesDebuggerTargetIfDebugOrVerboseOptionsAreTrueData()
        {
            yield return new object[] { true, true, true };
            yield return new object[] { false, true, true };
            yield return new object[] { true, false, true};
            yield return new object[] { false, false, false };
        }
    }
}
