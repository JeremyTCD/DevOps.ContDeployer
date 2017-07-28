using Microsoft.Extensions.Logging;

namespace JeremyTCD.PipelinesCE.Core
{
    public interface ILoggingConfig
    {
        void Configure(ILoggerFactory loggerFactory, PipelinesCEOptions pipelinesCEOptions);
    }
}
