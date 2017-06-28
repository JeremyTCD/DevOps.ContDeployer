using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;

namespace JeremyTCD.PipelinesCE
{
    public class PipelinesCERegistry : Registry
    {
        public PipelinesCERegistry()
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

            For<PipelinesCE>().Singleton().Use<PipelinesCE>();
            For<IPipelineRunner>().Singleton().Use<PipelineRunner>();
            For<IPipelineContext>().Singleton().Use<PipelineContext>();
            For<IStepContextFactory>().Singleton().Use<StepContextFactory>();
            For<IPipelineContextFactory>().Singleton().Use<PipelineContextFactory>();
        }
    }
}
