using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.PipelinesCE.Core
{
    public interface IPluginStartup
    {
        void ConfigureServices(IServiceCollection services);
    }
}
