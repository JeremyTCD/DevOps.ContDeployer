using StructureMap;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Core
{
    public class PipelineContext : IPipelineContext
    {
        public SharedPluginOptions SharedPluginOptions { get; set; }
        public IDictionary<string, object> SharedData { get; set; }
        public IDictionary<string, IContainer> PluginContainers { get; set; }
        public PipelinesCEOptions PipelinesCEOptions { get; set; }
    }
}
