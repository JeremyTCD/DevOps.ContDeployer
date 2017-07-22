using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.PipelinesCE.Tools
{
    public interface IPluginStartup
    {
        void ConfigureServices(IServiceCollection services);
    }
}
