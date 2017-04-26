using LibGit2Sharp;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public interface IPipelineContext
    {
        IStepFactory StepFactory { get; set; }
        IHttpManager HttpManager { get; set; }
        IProcessManager ProcessManager { get; set; }
        IRepository Repository { get; set; }
        IDictionary<string, object> SharedData { get; set; }
        LinkedList<IStep> Steps { get; set; }
        PipelineContextOptions PipelineContextOptions { get; set; }
        SharedOptions SharedOptions { get; set; }
    }
}
