using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Runtime.Loader;
using Xunit;
using System.Reflection;
using System.Linq;

namespace JeremyTCD.PipelinesCE.Core.Tests.IntegrationTests
{
    public class LoggingConfigIntegrationTests
    {
        private static readonly string _tempDir = Path.Combine(Path.GetTempPath(), $"{nameof(LoggingConfigIntegrationTests)}Temp");
        private MockRepository _mockRepository { get; }
        private DirectoryService _directoryService { get; }

        public LoggingConfigIntegrationTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default) { DefaultValue = DefaultValue.Mock };

            Mock<ILoggingService<DirectoryService>> mockDirectoryServiceLS = _mockRepository.Create<ILoggingService<DirectoryService>>();
            _directoryService = new DirectoryService(mockDirectoryServiceLS.Object);
        }

        /// <summary>
        /// Ensures that log targets are set up properly. This test also ensures that loggers from different AssemblyLoadContexts
        /// write to the same console and file (NLog concurrent writes is enabled). 
        /// 
        /// Note that output to the debugger cannot be tested as there is no way to redirect debugger output
        /// </summary>
        [Fact]
        public void Configure_ConfiguresLoggerFactoryToLogToSameConsoleAndLogFileFromMultipleContexts()
        {
            // Arrange
            string testLogFileName = "test.log";
            string testLogFile = $"{_tempDir}/{testLogFileName}";
            string testArchiveFileName = "testArchive.log";
            string testArchiveFile = $"{_tempDir}/{testArchiveFileName}"; // Provide a rooted archive file since rooting is tested in other tests
            string testLoggerName = "testLoggerName";
            string testLogMessage = "testLogMessage";

            // Custom context
            string solutionDir = Path.GetFullPath(typeof(LoggingConfigIntegrationTests).GetTypeInfo().Assembly.Location + "../../../../../../..");
            string stubProjectDir = "StubProject.Logger";
            string stubProjectAssembly = $"{solutionDir}/test/{stubProjectDir}/bin/Debug/netcoreapp2.0/{stubProjectDir}.dll"; // TODO incomplete
            BasicAssemblyLoadContext customContext = new BasicAssemblyLoadContext();
            Assembly customAssembly = customContext.LoadFromAssemblyPath(stubProjectAssembly);
            Type stubLoggerType = customAssembly.GetTypes().First();
            MethodInfo configureMethodInfo = stubLoggerType.GetMethod("Configure");
            MethodInfo logInformationMethodInfo = stubLoggerType.GetMethod("LogInformation");
            object stubLoggerInstance = Activator.CreateInstance(stubLoggerType);
            configureMethodInfo.Invoke(stubLoggerInstance, new object[] { testLogFile, testArchiveFile, testLoggerName });

            // Default context
            ILoggerFactory loggerFactory = new LoggerFactory();
            PipelinesCEOptions pipelinesCEOptions = new PipelinesCEOptions
            {
                FileLogging = true,
                LogFile = testLogFile,
                ArchiveFile = testArchiveFile
                // No way to redirect debugger output
                //,Verbose = true
            };

            Mock<ILoggingService<PathService>> mockPathServiceLS = _mockRepository.Create<ILoggingService<PathService>>();
            IPathService pathService = new PathService(mockPathServiceLS.Object);

            LoggingConfig loggingConfig = new LoggingConfig(pathService, null);
            loggingConfig.Configure(loggerFactory, pipelinesCEOptions);
            ILogger logger = loggerFactory.CreateLogger(testLoggerName);

            StringWriter writer = new StringWriter();
            Console.SetOut(writer);

            _directoryService.Empty(_tempDir);

            // Act
            logger.LogInformation(testLogMessage);
            logInformationMethodInfo.Invoke(stubLoggerInstance, new object[] { testLogMessage });

            // Assert
            loggerFactory.Dispose();
            writer.Dispose();
            string consoleResult = writer.ToString();
            string fileResult = File.ReadAllText(testLogFile);
            string expectedPattern = $@"\[.*?\]\[{testLoggerName}\]\[INFO\] {testLogMessage}\r\n\[.*?\]\[{testLoggerName}\]\[INFO\] {testLogMessage}";
            Assert.Matches(expectedPattern, fileResult);
            Assert.Matches(expectedPattern, consoleResult);
        }

        [Fact]
        public void Configure_ConfiguresRollingLogFiles()
        {
            // Arrange
            string testLogFileName = "test.log";
            string testLogFile = $"{_tempDir}/{testLogFileName}";
            string testArchiveFileName = "test.{####}.log";
            string testArchiveFile = $"{_tempDir}/{testArchiveFileName}";
            string testLoggerName = "testLoggerName";
            string testLogMessage = "testLogMessage";

            Mock<ILoggingService<PathService>> mockPathServiceLS = _mockRepository.Create<ILoggingService<PathService>>();
            IPathService pathService = new PathService(mockPathServiceLS.Object);
            LoggingConfig loggingConfig = new LoggingConfig(pathService, null);

            PipelinesCEOptions pipelinesCEOptions = new PipelinesCEOptions
            {
                FileLogging = true,
                LogFile = testLogFile,
                ArchiveFile = testArchiveFile                            
            };

            _directoryService.Empty(_tempDir);

            // Act
            File.Create(testLogFile).Dispose();

            ILoggerFactory loggerFactory1 = new LoggerFactory();
            loggingConfig.Configure(loggerFactory1, pipelinesCEOptions);
            ILogger logger1 = loggerFactory1.CreateLogger(testLoggerName);
            logger1.LogInformation(testLogMessage);
            loggerFactory1.Dispose();

            ILoggerFactory loggerFactory2 = new LoggerFactory();
            loggingConfig.Configure(loggerFactory2, pipelinesCEOptions);
            ILogger logger2 = loggerFactory2.CreateLogger(testLoggerName);
            logger2.LogInformation(testLogMessage);
            loggerFactory2.Dispose();

            // Assert
            Assert.True(File.Exists(testLogFile));
            Assert.True(File.Exists(testArchiveFile.Replace("{####}", "0000"))); // Archive file created on startup
            Assert.True(File.Exists(testArchiveFile.Replace("{####}", "0001"))); // Archive file name contains incremented number
        }

        private class BasicAssemblyLoadContext : AssemblyLoadContext
        {
            protected override Assembly Load(AssemblyName assemblyName)
            {
                return null;
            }
        }
    }
}
