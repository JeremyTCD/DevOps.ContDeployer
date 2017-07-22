using JeremyTCD.PipelinesCE.Core;
using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.PipelinesCE.Plugin.MSBuild
{
    public class MSBuildPluginStartup : IPluginStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Services for IMSBuildService, IDirectoryService and ILoggingService<> are added by default - see PipelinesCERegistry
        }
    }
}
