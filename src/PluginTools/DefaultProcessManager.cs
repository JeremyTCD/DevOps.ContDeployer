using System.Diagnostics;

namespace JeremyTCD.ContDeployer.PluginTools
{
    // TODO ExecuteAsync
    public class DefaultProcessManager : IProcessManager
    {
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

                return process.ExitCode;
            }
        }
    }
}
