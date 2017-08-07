using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace StubProject.Logger
{
    public class StubLogger
    {
        private ILogger _logger { get; set; }

        public void Configure(string logFile, string archiveFile, string loggerName)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddUtils();

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            IPathService pathService = serviceProvider.GetService<IPathService>();
            ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            PipelinesCEOptions pipelinesCEOptions = new PipelinesCEOptions
            {
                FileLogging = true,
                LogFile = logFile,
                ArchiveFile = archiveFile
            };

            LoggingConfig loggingConfig = new LoggingConfig(pathService, null);
            loggingConfig.Configure(loggerFactory, pipelinesCEOptions);

            _logger = loggerFactory.CreateLogger(loggerName);
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }
    }
}
