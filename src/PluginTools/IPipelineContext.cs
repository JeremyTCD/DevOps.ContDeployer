using JeremyTCD.DotNetCore.Utils;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.PluginTools
{
    public interface IPipelineContext
    {
        IProcessService ProcessService { get; set; }
        PipelineOptions PipelineOptions { get; set; }
        IDictionary<string, object> SharedData { get; set; }
    }
}
