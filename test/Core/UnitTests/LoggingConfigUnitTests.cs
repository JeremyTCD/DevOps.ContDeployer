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

        [Fact]
        public void CreateLoggingConfiguration_CreatesLoggingConfig()
        {
            //Arrange
            PipelinesCEOptions stubPipelinesCEOptions = new PipelinesCEOptions();

            LoggingConfig loggingConfig = new LoggingConfig(null, null);

            // Act
            LoggingConfiguration result = loggingConfig.CreateLoggingConfiguration(stubPipelinesCEOptions);

            // Assert
            _mockRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.Equal(1, result.ConfiguredNamedTargets.Count);
        }

        [Theory]
        [MemberData(nameof(SetsLogLevelAccordingToDebugAndVerboseOptionsData))]
        public void CreateLoggingConfiguration_SetsLogLevelAccordingToDebugAndVerboseOptions(bool debug, bool verbose, LogLevel expectedLogLevel)
        {
            // Arrange
            PipelinesCEOptions stubPipelinesCEOptions = new PipelinesCEOptions
            {
                Debug = debug,
                Verbose = verbose
            };

            LoggingConfig loggingConfig = new LoggingConfig(null, null);

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
        public void CreateLoggingConfiguration_CreatesFileTargetIfFileLoggingOptionsIsTrue()
        {
            // Arrange
            string testLogFile = "testLogFile";
            PipelinesCEOptions stubPipelinesCEOptions = new PipelinesCEOptions
            {
                FileLogging = true,
                LogFile = testLogFile
            };

            Mock<IPathService> mockPathService = _mockRepository.Create<IPathService>();
            mockPathService.Setup(p => p.IsPathRooted(testLogFile)).Returns(true);

            LoggingConfig loggingConfig = new LoggingConfig(mockPathService.Object, null);

            // Act
            LoggingConfiguration result = loggingConfig.CreateLoggingConfiguration(stubPipelinesCEOptions);

            // Assert
            _mockRepository.VerifyAll();
            Assert.True(result.ConfiguredNamedTargets.Any(t => t.GetType() == typeof(FileTarget)));
        }

        [Fact]
        public void CreateLoggingConfiguration_RootsLogFileIfItIsNotRooted()
        {
            //Arrange
            string testLogFile = "testLogFile";
            string testLogFileRooted = $"C:/{testLogFile}";
            string testProjectFile = "C:/testProjectFile";
            PipelinesCEOptions stubPipelinesCEOptions = new PipelinesCEOptions
            {
                FileLogging = true,
                LogFile = testLogFile,
                ProjectFile = testProjectFile
            };
            DirectoryInfo stubDirectoryInfo = new DirectoryInfo("C:/");
            string testProjectDir = stubDirectoryInfo.FullName;

            Mock<IPathService> mockPathService = _mockRepository.Create<IPathService>();
            mockPathService.Setup(p => p.IsPathRooted(testLogFile)).Returns(false);
            mockPathService.Setup(p => p.Combine(testProjectDir, testLogFile)).Returns(testLogFileRooted);

            Mock<IDirectoryService> mockDirectoryService = _mockRepository.Create<IDirectoryService>();
            mockDirectoryService.Setup(d => d.GetParent(testProjectFile)).Returns(stubDirectoryInfo);

            LoggingConfig loggingConfig = new LoggingConfig(mockPathService.Object, mockDirectoryService.Object);

            // Act
            LoggingConfiguration result = loggingConfig.CreateLoggingConfiguration(stubPipelinesCEOptions);

            // Assert
            _mockRepository.VerifyAll();
            FileTarget fileTarget = result.ConfiguredNamedTargets.Single(t => t.GetType() == typeof(FileTarget)) as FileTarget;
            Assert.NotNull(fileTarget);
            Assert.Equal($"'{testLogFileRooted}'", fileTarget.FileName.ToString());
        }

        [Theory]
        [MemberData(nameof(CreatesDebuggerTargetIfDebugOrVerboseOptionsAreTrueData))]
        public void CreateLoggingConfiguration_CreatesDebuggerTargetIfDebugOrVerboseOptionsAreTrue(bool debug, bool verbose)
        {
            // Arrange
            string testLogFile = "testLogFile";
            PipelinesCEOptions stubPipelinesCEOptions = new PipelinesCEOptions
            {
                Debug = debug,
                Verbose = verbose,
                LogFile = testLogFile
            };

            LoggingConfig loggingConfig = new LoggingConfig(null, null);

            // Act
            LoggingConfiguration result = loggingConfig.CreateLoggingConfiguration(stubPipelinesCEOptions);

            // Assert
            _mockRepository.VerifyAll();
            Assert.True(result.ConfiguredNamedTargets.Any(r => r.GetType() == typeof(DebuggerTarget)));
        }

        public static IEnumerable<object[]> CreatesDebuggerTargetIfDebugOrVerboseOptionsAreTrueData()
        {
            yield return new object[] { true, true};
            yield return new object[] { false, true};
            yield return new object[] { true, false};
        }
    }
}
