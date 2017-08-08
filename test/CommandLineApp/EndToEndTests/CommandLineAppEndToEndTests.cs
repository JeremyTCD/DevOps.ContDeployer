using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.Core;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Logging;
using Moq;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using System;
using System.IO;
using System.Reflection;
using Xunit;
using LogLevel = NLog.LogLevel;

namespace JeremyTCD.PipelinesCE.CommandLineApp.Tests.EndToEndTests
{
    /// <summary>
    /// Tests to ensure that CommandLineApp is configured to build correctly.
    /// </summary>
    public class CommandLineAppEndToEndTests
    {
        private MSBuildService _msBuildService { get; }
        private MockRepository _mockRepository { get; }
        private DirectoryService _directoryService { get; }
        private ProcessService _processService { get; }
        private LoggerFactory _loggerFactory { get; }
        private string _tempDir { get; } = Path.Combine(Path.GetTempPath(), $"{nameof(CommandLineAppEndToEndTests)}Temp");

        public CommandLineAppEndToEndTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
            // TODO log to debug output
            _directoryService = new DirectoryService(_mockRepository.Create<ILoggingService<DirectoryService>>().Object);
            _loggerFactory = new LoggerFactory();
            ConfigureLoggerFactory(_loggerFactory);
            ILogger<ProcessService> logger = _loggerFactory.CreateLogger<ProcessService>();
            _processService = new ProcessService(new LoggingService<ProcessService>(logger));
            _msBuildService = new MSBuildService(_processService, _mockRepository.Create<ILoggingService<MSBuildService>>().Object);
        }

        [Fact]
        public void CommandLineApp_ProjectDeploymentConfiguredCorrectly()
        {
            // Arrange
            string rid = RuntimeEnvironment.GetRuntimeIdentifier();
            string solutionDir = Path.GetFullPath(typeof(CommandLineAppEndToEndTests).GetTypeInfo().Assembly.Location + "../../../../../../..");
            string claProjectAbsDir = $"{solutionDir}/src/CommandLineApp";
            string claProjectFilePath = $"{claProjectAbsDir}/JeremyTCD.PipelinesCE.CommandLineApp.csproj";
            string exePath = $"{claProjectAbsDir}/bin/Release/netcoreapp2.0/{rid}/PipelinesCE.exe";
            // This magic string is unavoidable, can't reference this assembly since it is to be build and loaded by PipelinesCE
            string stubProjectDir = "StubProject.PipelinesCEConfig";
            string stubProjectAbsSrcDir = $"{solutionDir}/test/{stubProjectDir}";
            string stubProjectFilePath = $"{_tempDir}/{stubProjectDir}.csproj";
            _directoryService.Empty(_tempDir);
            _directoryService.Copy(stubProjectAbsSrcDir, _tempDir, excludePatterns: new string[] { "^bin$", "^obj$" });
            _msBuildService.ConvertProjectReferenceRelPathsToAbs(stubProjectFilePath, stubProjectAbsSrcDir);

            _directoryService.SetCurrentDirectory(claProjectAbsDir);

            ThreadSpecificStringWriter tssw = new ThreadSpecificStringWriter();
            Console.SetOut(tssw);

            // Act
            _msBuildService.Build(claProjectFilePath, $"/t:Restore,Publish /p:Configuration=Release,RuntimeIdentifier={rid}");
            int exitCode = _processService.Run(exePath, $"{Strings.CommandName_Run} --{Strings.OptionLongName_ProjectFile} {stubProjectFilePath} --{Strings.OptionLongName_Verbose}");

            // Assert 
            Assert.Equal(0, exitCode);
            _loggerFactory.Dispose();
            tssw.Dispose();
            string output = tssw.ToString();
            Assert.Contains(string.Format(ConfigHost.Strings.Log_FinishedRunningPipeline, "Stub"), output);
            Assert.Contains(string.Format(ConfigHost.Strings.Log_FinishedRunningPlugin, "StubPlugin"), output);
        }

        private void ConfigureLoggerFactory(ILoggerFactory loggerFactory)
        {
            string layout = "[${longdate}][${logger}][${level: uppercase = true}] ${message}";
            LoggingConfiguration config = new LoggingConfiguration();

            // Console
            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget
            {
                Name = nameof(ConsoleTarget),
                Layout = layout
            };
            config.AddTarget(consoleTarget);
            LoggingRule consoleRule = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(consoleRule);

            // Debugger
            DebuggerTarget debuggerTarget = new DebuggerTarget
            {
                Name = nameof(DebugTarget),
                Layout = layout
            };
            config.AddTarget(debuggerTarget);
            LoggingRule debugRule = new LoggingRule("*", LogLevel.Debug, debuggerTarget);
            config.LoggingRules.Add(debugRule);

            loggerFactory.
                AddNLog().
                ConfigureNLog(config);
        }
    }
}