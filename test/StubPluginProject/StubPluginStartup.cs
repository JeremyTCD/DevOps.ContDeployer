using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.PipelinesCE.Tests.StubPluginProject
{
    public class StubPluginStartup : IPluginStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IStubService, StubService>();
        }
    }
}
