using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using StructureMap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit;

namespace JeremyTCD.PipelinesCE.CommandLineApp.Tests.IntegrationTests
{
    public class ProgramIntegrationTests
    {
        private MockRepository _mockRepository { get; }
        private string _tempDir { get; } = Path.Combine(Path.GetTempPath(), $"{nameof(ProgramIntegrationTests)}Temp");
        private DirectoryService _directoryService { get; }
        private MSBuildService _msBuildService { get; }

        public ProgramIntegrationTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
            _directoryService = new DirectoryService(_mockRepository.Create<ILoggingService<DirectoryService>>().Object);
            _msBuildService = new MSBuildService(null, _mockRepository.Create<ILoggingService<MSBuildService>>().Object);
        }

        /// <summary>
        /// Test to ensure that <see cref="CommandLineAppRegistry"/> configures services correctly
        /// </summary>
        [Fact]
        public void Main_RunsPipeline()
        {
            // Arrange
            string solutionDir = Path.GetFullPath(typeof(ProgramIntegrationTests).GetTypeInfo().Assembly.Location + "../../../../../../..");
            string stubProjectDir = "StubProject.PipelinesCEConfig";
            string stubProjectAbsSrcDir = $"{solutionDir}/test/{stubProjectDir}";
            string stubProjectFile = $"{stubProjectDir}.csproj";
            string stubProjectFilePath = $"{_tempDir}/{stubProjectFile}";
            string stubPipelineName = "Stub";
            _directoryService.Empty(_tempDir);
            _directoryService.Copy(stubProjectAbsSrcDir, _tempDir, excludePatterns: new string[] { "^bin$", "^obj$" });
            _msBuildService.ConvertProjectReferenceRelPathsToAbs(stubProjectFilePath, stubProjectAbsSrcDir);

            // Act
            int exitCode = Program.Main(new string[] { Strings.CommandName_Run,
                $"-{Strings.OptionShortName_Verbose}",
                $"-{Strings.OptionShortName_Debug}",
                $"-{Strings.OptionShortName_Project}", stubProjectFilePath,
                $"-{Strings.OptionShortName_Pipeline}", stubPipelineName });

            // Assert 
            Assert.Equal(0, exitCode);
        }
    }
}
