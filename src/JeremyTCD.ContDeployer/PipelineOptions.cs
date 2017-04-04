using JeremyTCD.ContDeployer.PluginTools;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer
{
    public class PipelineOptions
    {
        public IEnumerable<PipelineStep> PipelineSteps { get; set; }
    }
}
