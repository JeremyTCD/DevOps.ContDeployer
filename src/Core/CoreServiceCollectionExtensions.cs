using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JeremyTCD.PipelinesCE.Core
{
    public static class CoreServiceCollectionExtensions
    {
        public static void AddCore(this IServiceCollection services)
        {
            services.TryAdd(ServiceDescriptor.Singleton<IStepContextFactory, StepContextFactory>());
            services.TryAdd(ServiceDescriptor.Singleton<IPipelineContextFactory, PipelineContextFactory>());
            services.TryAdd(ServiceDescriptor.Singleton<ILoggingConfig, LoggingConfig>());
        }
    }
}
