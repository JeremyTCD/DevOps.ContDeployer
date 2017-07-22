﻿using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Reflection;
using Microsoft.DotNet.PlatformAbstractions;
using System.Xml;
using Xunit;
using System.Linq;

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
            _loggerFactory.
                AddDebug(LogLevel.Debug);
            ILogger<ProcessService> logger = _loggerFactory.CreateLogger<ProcessService>();
            _processService = new ProcessService(new LoggingService<ProcessService>(logger));
            _msBuildService = new MSBuildService(_processService, _mockRepository.Create<ILoggingService<MSBuildService>>().Object);

            _directoryService.DeleteIfExists(_tempDir, true);
            _directoryService.Create(_tempDir);
            _directoryService.SetCurrentDirectory(_tempDir);
        }

        [Fact]
        public void CommandLineApp_ProjectDeploymentConfiguredCorrectly()
        {
            // Arrange
            string rid = RuntimeEnvironment.GetRuntimeIdentifier();
            string solutionDir = Path.GetFullPath(typeof(CommandLineAppEndToEndTests).GetTypeInfo().Assembly.Location + "../../../../../../..");
            string claProjectAbsDir = Path.GetFullPath(solutionDir + "/src/CommandLineApp");
            string exePath = $"{claProjectAbsDir}/bin/Release/netcoreapp2.0/{rid}/PipelinesCE.exe";
            // This magic string is unavoidable, can't reference this assembly since it is to be build and loaded by PipelinesCE
            string stubProjectDir = "StubProject.PipelinesCEConfig";
            string stubProjectAbsSrcDir = $"{solutionDir}/test/{stubProjectDir}";
            string stubProjectFile = $"{stubProjectDir}.csproj";
            string stubProjectFilePath = $"{_tempDir}/{stubProjectFile}";
            // Copy stub project to temp dir
            _directoryService.Copy(stubProjectAbsSrcDir, _tempDir, excludePatterns: new string[] { "^bin$", "^obj$" });
            ConvertProjectReferenceRelPathsToAbs(stubProjectFilePath, stubProjectAbsSrcDir);

            _directoryService.SetCurrentDirectory(claProjectAbsDir);

            _msBuildService.Build(switches: $"/t:Restore,Publish /p:Configuration=Release,RuntimeIdentifier={rid}");
            _directoryService.SetCurrentDirectory(_tempDir);
            int exitCode = _processService.Run(exePath, $"{Strings.CommandName_Run} --{Strings.OptionLongName_Project} {stubProjectFile} --{Strings.OptionLongName_Verbose}");

            // Assert 
            Assert.Equal(0, exitCode);
            _loggerFactory.Dispose();

            string logFile = Directory.GetFiles(_tempDir, PipelineOptions.LogFileFormat.Replace("{Date}", "*")).FirstOrDefault();
            Assert.NotNull(logFile);
            string output = File.ReadAllText(logFile);
            Assert.Contains(string.Format(PipelineRunner.Strings.Log_FinishedRunningPipeline, "\"Stub\""), output);
            Assert.Contains(string.Format(PipelineRunner.Strings.Log_FinishedRunningPlugin, "\"StubPlugin\""), output);
        }

        private void ConvertProjectReferenceRelPathsToAbs(string projectFile, string projectDir)
        {
            XmlDocument doc = new XmlDocument();

            FileStream stream = new FileStream(projectFile, FileMode.Open);
            doc.Load(stream);
            stream.Dispose();

            foreach (XmlNode node in doc.GetElementsByTagName("ProjectReference"))
            {
                node.Attributes["Include"].Value = Path.Combine(projectDir, node.Attributes["Include"].Value);
            }

            stream = new FileStream(projectFile, FileMode.Create);
            doc.Save(stream);
            stream.Dispose();
        }
    }
}
