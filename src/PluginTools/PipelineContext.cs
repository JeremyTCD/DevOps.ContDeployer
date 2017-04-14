using LibGit2Sharp;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public class PipelineContext
    {
        public IProcessManager ProcessManager { get; set; }
        public IRepository Repository { get; set; }
        public Dictionary<string, object> SharedData { get; set; }
        public LinkedList<Step> Steps { get; set; }
    }
}
