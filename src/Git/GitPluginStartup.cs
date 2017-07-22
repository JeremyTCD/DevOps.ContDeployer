using JeremyTCD.PipelinesCE.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.PipelinesCE.Plugin.Git
{
    public class GitPluginStartup : IPluginStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IRepositoryFactory, RepositoryFactory>();
        }
    }
}
