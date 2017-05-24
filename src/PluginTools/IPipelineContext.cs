using JeremyTCD.DotNetCore.Utils;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.PluginTools
{
    public interface IPipelineContext
    {
        IProcessManager ProcessManager { get; set; }
        IDictionary<string, object> SharedData { get; set; }
        SharedOptions SharedOptions { get; set; }
    }
}
