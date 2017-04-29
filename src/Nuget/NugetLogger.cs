using Microsoft.Extensions.Logging;
using NuGet.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyTCD.ContDeployer.Plugin.Nuget
{
    /// <summary>
    /// Pipes <see cref="NuGet.Common.ILogger"/> logs to an <see cref="Microsoft.Extensions.Logging.ILogger"/>
    /// instance.
    /// </summary>
    public class NugetLogger : NuGet.Common.ILogger
    {
        private ILogger<NugetLogger> _logger { get; }

        /// <summary>
        /// Creates a <see cref="NugetLogger"/>instance.
        /// </summary>
        /// <param name="logger"></param>
        public NugetLogger(ILogger<NugetLogger> logger)
        {
            _logger = logger;
        }

        public void LogDebug(string data)
        {
            _logger.LogDebug(data);
        }

        public void LogError(string data)
        {
            _logger.LogError(data);
        }

        public void LogErrorSummary(string data)
        {
            _logger.LogError(data);
        }

        public void LogInformation(string data)
        {
            _logger.LogInformation(data);
        }

        public void LogInformationSummary(string data)
        {
            _logger.LogInformation(data);
        }

        public void LogMinimal(string data)
        {
            _logger.LogCritical(data);
        }

        public void LogVerbose(string data)
        {
            _logger.LogTrace(data);
        }

        public void LogWarning(string data)
        {
            _logger.LogWarning(data);
        }
    }
}
