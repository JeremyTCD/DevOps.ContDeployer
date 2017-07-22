using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.Tools;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;

namespace JeremyTCD.PipelinesCE.ConfigHost
{
    public class ConfigHostRegistry : Registry
    {
        public ConfigHostRegistry()
        {
            IServiceCollection services = new ServiceCollection();
            services.
                AddLogging().
                AddOptions();
            this.Populate(services);

            For<IAssemblyService>().Singleton().Use<AssemblyService>();
            For<IPathService>().Singleton().Use<PathService>();
            For<IDirectoryService>().Singleton().Use<DirectoryService>();
            For<IMSBuildService>().Singleton().Use<MSBuildService>();
            For<IActivatorService>().Singleton().Use<ActivatorService>();
            For(typeof(ILoggingService<>)).Singleton().Use(typeof(LoggingService<>));
            For<IProcessService>().Singleton().Use<ProcessService>();
            For<IDependencyContextService>().Singleton().Use<DependencyContextService>();
            For<IFileService>().Singleton().Use<FileService>();
            For<IAssemblyLoadContextService>().Singleton().Use<AssemblyLoadContextService>();

            For<ConfigHostCore>().Singleton().Use<ConfigHostCore>();
            For<IPipelineLoader>().Singleton().Use<PipelineLoader>();
            For<IPipelineRunner>().Singleton().Use<PipelineRunner>();
            For<IStepContextFactory>().Singleton().Use<StepContextFactory>();
            For<IPipelineContextFactory>().Singleton().Use<PipelineContextFactory>();
        }
    }
}