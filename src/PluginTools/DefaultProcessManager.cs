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
        public int Execute(string fileName, string arguments)
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
                process.WaitForExit();

                return process.ExitCode;
            }
        }
    }
}
