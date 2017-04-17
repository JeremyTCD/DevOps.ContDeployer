using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace JeremyTCD.ContDeployer.PluginTools
{
    // TODO ExecuteAsync
    public class ProcessManager : IProcessManager
    {
        private ILogger<ProcessManager> _logger { get; }
        private SharedOptions _sharedOptions { get; }

        public ProcessManager(ILogger<ProcessManager> logger, IOptions<SharedOptions> sharedOptionsAccessor)
        {
            _logger = logger;
            _sharedOptions = sharedOptionsAccessor.Value;
        }

        /// <summary>
        /// Runs process synchronously
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="arguments"></param>
        /// <returns>
        /// Exit code
        /// </returns>
        public int Execute(string fileName, string arguments, int timeoutMillis = int.MaxValue, 
            bool executeOnDryRun = false)
        {
            if (_sharedOptions.DryRun && !executeOnDryRun)
            {
                _logger.LogInformation($"Dry run \"{fileName} {arguments}\"");

                return 0;
            }
            else
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
}
