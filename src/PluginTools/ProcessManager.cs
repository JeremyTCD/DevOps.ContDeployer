using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace JeremyTCD.PipelinesCE.PluginTools
{
    // TODO ExecuteAsync
    public class ProcessManager : IProcessManager
    {
        private ILogger<ProcessManager> _logger { get; }

        public ProcessManager(ILogger<ProcessManager> logger)
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
        public int Run(string fileName, string arguments, int timeoutMillis = int.MaxValue)
        {
            // TODO output not reaching application
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.OutputDataReceived += new DataReceivedEventHandler(
                    (sender, eventArgs) =>
                    {
                        if (eventArgs.Data != null)
                        {
                            _logger.LogInformation(eventArgs.Data);
                        }
                    }
                );
                process.ErrorDataReceived += new DataReceivedEventHandler(
                    (sender, eventArgs) =>
                    {
                        if (eventArgs.Data != null)
                        {
                            _logger.LogError(eventArgs.Data);
                        }
                    }
                );

                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                process.WaitForExit(timeoutMillis);

                return process.ExitCode;
            }
        }
    }
}
