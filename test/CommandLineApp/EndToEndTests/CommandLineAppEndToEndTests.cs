using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Reflection;
using System.Xml;
using Xunit;

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
        private string _tempDir { get; } = Path.Combine(Path.GetTempPath(), $"{nameof(Program)}Temp");

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

            _directoryService.Delete(_tempDir, true);
            _directoryService.Create(_tempDir);
            _directoryService.SetCurrentDirectory(_tempDir);
        }

        [Fact]
        public void PublishTarget_PublishesWorkingPipelinesCEExecutable()
        {
            // Arrange
            string solutionDir = Path.GetFullPath(typeof(Program).GetTypeInfo().Assembly.Location + "../../../../../../..");
            string claProjectAbsDir = Path.GetFullPath(solutionDir + "/src/CommandLineApp");
            string exePath = Path.GetFullPath(claProjectAbsDir + "/bin/Release/netcoreapp1.1/win10-x64/PipelinesCE.exe");
            // This magic string is unavoidable, can't reference this assembly since it is to be build and loaded by PipelinesCE
            string stubProjectDir = "StubPipelinesCEProject";
            string stubProjectAbsSrcDir = Path.GetFullPath(solutionDir + $"/test/{stubProjectDir}");
            string stubProjectFile = $"JeremyTCD.PipelinesCE.Tests.{stubProjectDir}.csproj";
            string stubProjectFilePath = Path.Combine(_tempDir, stubProjectFile);
            // Copy stub project to temp dir
            _directoryService.Copy(stubProjectAbsSrcDir, _tempDir, excludePatterns: new string[] { "^bin$", "^obj$" });
            ConvertProjectReferenceRelPathsToAbs(stubProjectFilePath, stubProjectAbsSrcDir);

            _directoryService.SetCurrentDirectory(claProjectAbsDir);

            // Act
            // TODO for this test to pass on all platforms, RuntimeIdentifier must be for the current OS
            _msBuildService.Build(switches: "/t:Publish /p:Configuration=Release,RuntimeIdentifier=win10-x64");
            _directoryService.SetCurrentDirectory(_tempDir);
            int exitCode = _processService.Run(exePath, $"{Strings.CommandName_Run} --{Strings.OptionLongName_Project} {stubProjectFile} --{Strings.OptionLongName_Verbose}");

            // Assert 
            Assert.Equal(0, exitCode);
            _loggerFactory.Dispose();

            CommandLineAppOptions options = new CommandLineAppOptions();
            string output = File.ReadAllText(options.LogFileFormat.Replace("{Date}", DateTime.Today.ToString("yyyyMMdd")));
            //Assert.Contains(string.Format(JeremyTCD.PipelinesCE.Strings.Log_PipelineComplete, "\"Stub\""), output);
            //Assert.Contains(string.Format(JeremyTCD.PipelinesCE.Strings.Log_PluginComplete, "\"StubPlugin\""), output);
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
