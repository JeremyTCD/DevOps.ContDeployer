using StructureMap;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Core
{
    public interface IPipelineContext
    {
        SharedPluginOptions SharedPluginOptions { get; set; }
        IDictionary<string, object> SharedData { get; set; }
        PipelinesCEOptions PipelinesCEOptions { get; set; }
        IDictionary<string, IContainer> PluginContainers { get; set; }
    }
}
