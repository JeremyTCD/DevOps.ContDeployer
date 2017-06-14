using JeremyTCD.DotNetCore.Utils;
using Moq;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace JeremyTCD.PipelinesCE.CommandLineApp.Tests.EndToEndTests
{
    /// <summary>
    /// Tests to ensure that <see cref="Program.Main(string[])"/> behaves correctly as well as that
    /// CommandLineApp is configured to build correctly.
    /// </summary>
    public class CommandLineAppEndToEndTests
    {
        private MSBuildService _msBuildService { get; }
        private MockRepository _mockRepository { get; }
        private DirectoryService _directoryService { get; }
        private ProcessService _processService { get; }
        private string _tempDir { get; } = Path.Combine(Path.GetTempPath(), $"{nameof(CommandLineApp)}Temp");

        public CommandLineAppEndToEndTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
            _processService = new ProcessService(_mockRepository.Create<ILoggingService<ProcessService>>().Object);
            _msBuildService = new MSBuildService(_processService, _mockRepository.Create<ILoggingService<MSBuildService>>().Object);
            _directoryService = new DirectoryService(_mockRepository.Create<ILoggingService<DirectoryService>>().Object);

            _directoryService.Delete(_tempDir, true);
            _directoryService.Create(_tempDir);
        }

        [Fact]
        public void CommandLineApp_RunsPipelineIfCommandLineAppMainBehavesCorrectly()
        {
            // Arrange
            string solutionDir = Path.GetFullPath(typeof(CommandLineApp).GetTypeInfo().Assembly.Location + "../../../../../../../");
            string claProjectAbsDir = Path.GetFullPath(solutionDir + "src/CommandLineApp");
            string exePath = "";
            // This magic string is unavoidable, can't reference this assembly since it is to be build and loaded by PipelinesCE
            string stubProjectDir = "StubPipelinesCEProject";
            string stubProjectAbsSrcDir = Path.GetFullPath(solutionDir + $"test/{stubProjectDir}");
            string stubProjectFile = Path.Combine(_tempDir, $"JeremyTCD.PipelinesCE.Tests.{stubProjectDir}.csproj");

            // Publish PipelinesCE.exe
            // TODO for this test to pass on all platforms, RuntimeIdentifier must be for the current OS
            _directoryService.SetCurrentDirectory(claProjectAbsDir);
            _msBuildService.Build(switches: "/t:Publish /p:Configuration=Release,RuntimeIdentifier=win10-x64");
            // Copy stub project to temp dir
            _directoryService.Copy(stubProjectAbsSrcDir, _tempDir, excludePatterns: new string[] { "^bin$", "^obj$" });

            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            // Act
            _directoryService.SetCurrentDirectory(_tempDir);
            int exitCode = _processService.Run(exePath, "");

            // Assert that expected messages are printed to console
            Assert.Equal(0, exitCode);
            stringWriter.Dispose();
            string output = stringWriter.ToString();
            //Assert.Contains(string.Format(Strings.Log_PipelineComplete, "Stub"), output);
            //Assert.Contains(string.Format(Strings.Log_PluginComplete, "StubPlugin"), output);
        }

        // Ensure that exceptions that bubble up to main are logged 
        // Ensure that verbosity is processed correctly by main (verboseoff?)
    }
}
