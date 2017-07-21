using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;

namespace JeremyTCD.PipelinesCE.PipelineRunner
{
    public class PipelineRunnerRegistry : Registry
    {
        public PipelineRunnerRegistry()
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

            For<Root>().Singleton().Use<Root>();
            For<ILoader>().Singleton().Use<Loader>();
            For<IRunner>().Singleton().Use<Runner>();
            For<IStepContextFactory>().Singleton().Use<StepContextFactory>();
            For<IPipelineContextFactory>().Singleton().Use<PipelineContextFactory>();
        }
    }
}