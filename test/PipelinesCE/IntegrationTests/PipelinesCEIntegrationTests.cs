using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using StructureMap;
using System;
using System.IO;
using System.Reflection;
using System.Xml;
using Xunit;

namespace JeremyTCD.PipelinesCE.Tests.IntegrationTests
{
    public class PipelinesCEIntegrationTests
    {
        private MockRepository _mockRepository { get; }
        private DirectoryService _directoryService { get; }
        private string _tempDir { get; } = Path.Combine(Path.GetTempPath(), $"{nameof(PipelinesCE)}Temp");
        private IContainer _container { get; }
        private ILoggerFactory _loggerFactory { get; }

        public PipelinesCEIntegrationTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
            _directoryService = new DirectoryService(_mockRepository.Create<ILoggingService<DirectoryService>>().Object);
            _container = CreateContainer();
            _loggerFactory = _container.GetInstance<ILoggerFactory>();
            _loggerFactory.AddConsole(LogLevel.Information);
            _directoryService.Delete(_tempDir, true);
            _directoryService.Create(_tempDir);
            _directoryService.SetCurrentDirectory(_tempDir);
        }

        /// <summary>
        /// This test covers the full breadth of <see cref="PipelinesCE"/>. In particular, it verifies that
        /// <see cref="PipelinesCEServiceCollectionExtensions.AddPipelinesCE(IServiceCollection)"/> configures default PipelinesCE
        /// services correctly as well as that <see cref="PipelinesCE.Run(PipelineOptions)"/> configures per plugin
        /// services correctly.
        /// </summary>
        [Fact]
        public void Run_RunsPipelineIfServicesAreConfiguredCorrectly()
        {
            // Arrange
            string solutionDir = Path.GetFullPath(typeof(PipelinesCEIntegrationTests).GetTypeInfo().Assembly.Location + "../../../../../../../");
            // This magic string is unavoidable, can't reference this assembly since it is to be build and loaded by PipelinesCE
            string projectDir = "StubPipelinesCEProject";
            string projectAbsSrcDir = Path.GetFullPath(solutionDir + $"test/{projectDir}");
            string projectAbsDestDir = Path.Combine(_tempDir, projectDir);
            // Copy stub project to temp dir
            _directoryService.Copy(projectAbsSrcDir, projectAbsDestDir, excludePatterns: new string[] { "^bin$", "^obj$" });
            // Replace relevant relative paths
            string ProjectFile = Path.Combine(projectAbsDestDir, $"JeremyTCD.PipelinesCE.Tests.{projectDir}.csproj");
            ConvertProjectReferenceRelPathsToAbs(ProjectFile, projectAbsSrcDir);

            PipelinesCE pipelinesCE = _container.GetInstance<PipelinesCE>();

            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            // Act
            pipelinesCE.Run(new PipelineOptions { Project = ProjectFile });

            // Assert
            _loggerFactory.Dispose();
            stringWriter.Dispose();
            string output = stringWriter.ToString();
            // If these two logs were written to console, ServiceCollectionExtensions.AddPipelinesCE configures services correctly and PipelinesCE.Run 
            // configures per plugin services correctly
            Assert.Contains(string.Format(Strings.Log_PipelineComplete, "Stub"), output);
            Assert.Contains(string.Format(Strings.Log_PluginComplete, "StubPlugin"), output);
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

        private IContainer CreateContainer()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddPipelinesCE();
            IContainer mainContainer = new Container();
            mainContainer.Populate(services);

            return mainContainer;
        }
    }
}
