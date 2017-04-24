using LibGit2Sharp;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public interface IPipelineContext
    {
        IHttpManager HttpManager { get; set; }
        IProcessManager ProcessManager { get; set; }
        IRepository Repository { get; set; }
        Dictionary<string, object> SharedData { get; set; }
        LinkedList<IStep> Steps { get; set; }
    }
}
