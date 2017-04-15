using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace JeremyTCD.ContDeployer.PluginTools
{
    // TODO ExecuteAsync
    public class DefaultProcessManager : IProcessManager
    {
        private ILogger<DefaultProcessManager> _logger { get; }

        public DefaultProcessManager(ILogger<DefaultProcessManager> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Runs process synchronously
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="arguments"></param>
        /// <returns>
        /// Exit code
        /// </returns>
        public int Execute(string fileName, string arguments, int timeoutMillis = int.MaxValue)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                CreateNoWindow = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();
                process.WaitForExit(timeoutMillis);

                _logger.LogInformation($"Successfully executed \"{fileName} {arguments}\"");
                return process.ExitCode;
            }
        }
    }
}
