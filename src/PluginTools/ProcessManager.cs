using System.Diagnostics;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public class ProcessManager : IProcessManager
    {
        public void Execute(string fileName, string arguments)
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
            }
        }
    }
}
