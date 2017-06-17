using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace JeremyTCD.PipelinesCE
{
    public static class PipelinesCEServiceCollectionExtensions
    {
        public static void AddPipelinesCE(this IServiceCollection services)
        {
            services.
                AddLogging().
                AddOptions().
                AddSingleton<IAssemblyService, AssemblyService>().
                AddSingleton<IPathService, PathService>().
                AddSingleton<IDirectoryService, DirectoryService>().
                AddSingleton<IMSBuildService, MSBuildService>().
                AddSingleton<IActivatorService, ActivatorService>().
                AddSingleton(typeof(ILoggingService<>), typeof(LoggingService<>)).
                AddSingleton<HttpClient>();

            services.
                AddSingleton<PipelinesCE>().
                AddSingleton<IProcessService, ProcessService>().
                AddSingleton<IPipelineRunner, PipelineRunner>().
                AddSingleton<IPipelineContext, PipelineContext>().
                AddSingleton<IStepContextFactory, StepContextFactory>().
                AddSingleton<IPipelineContextFactory, PipelineContextFactory>();
        }
    }
}
