using JeremyTCD.PipelinesCE.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.PipelinesCE.Plugin.GitHub
{
    public class GitHubPluginStartup : IPluginStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider => new GitHubClientFactory("https://api.gitHub.com", nameof(PipelinesCE)));
        }
    }
}
