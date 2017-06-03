using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System.IO;
using System.Reflection;
using Xunit;

namespace JeremyTCD.PipelinesCE.Tests.IntegrationTests
{
    /// <summary>
    /// The goal of these tests is to ensure that <see cref="ServiceCollectionExtensions"/> configures PipelinesCE
    /// services properly.
    /// </summary>
    public class PipelinesCEIntegrationTests
    {
        private DirectoryService _directoryService = new DirectoryService();
        private string _tempDir { get; } = Path.Combine(Path.GetTempPath(), $"{nameof(PipelinesCE)}temp");
        private IContainer _container { get; } 

        private string _projectFileName { get; } = "PipelinesCE.csproj";
        private string _assemblyName { get; } = "JeremyTCD.Test.dll";

        public PipelinesCEIntegrationTests()
        {
            _container = CreateContainer();
            _directoryService.Delete(_tempDir, true);
            _directoryService.Create(_tempDir);
            _directoryService.SetCurrentDirectory(_tempDir);
        }

        [Fact]
        public void Run_ConfiguresContainerAndCallsPipelineRunnerRunWithExpectedPipelineInstance()
        {
            // Copy both stub projects to tempdir
            string currentAssemblyPath = Directory.GetParent(typeof(PipelinesCEIntegrationTests).GetTypeInfo().Assembly.Location).FullName;
            string stubPipelinesCEProjectPath = Path.Combine(currentAssemblyPath, $"../../../../StubPipelinesCEProject");
            string stubPluginProjectPath = Path.Combine(currentAssemblyPath, $"../../../../StubPluginProject");

            // TODO copy only shallow copies contents of specified directory, it should copy the entire directory
            // allow for ignoring of specific files/folders such as bin and obj in this case
            _directoryService.Copy(stubPipelinesCEProjectPath, _tempDir);
            _directoryService.Copy(stubPluginProjectPath, _tempDir);

            // Create child container, override PipelineRunner (for now)
            // Run PipelinesCE in folder
        }

        public IContainer CreateContainer()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddPipelinesCE();
            IContainer mainContainer = new Container();
            mainContainer.Populate(services);

            return mainContainer;
        }
    }
}
