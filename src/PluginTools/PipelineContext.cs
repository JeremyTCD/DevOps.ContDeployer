using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.PluginTools
{
    public class PipelineContext : IPipelineContext
    {
        public PipelineOptions PipelineOptions { get; set; }
        public IDictionary<string, object> SharedData { get; set; }
    }
}
