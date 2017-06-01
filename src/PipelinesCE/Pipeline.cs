using JeremyTCD.PipelinesCE.PluginTools;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE
{
    public class Pipeline
    {
        public IEnumerable<IStep> Steps { get; set; }
        public PipelineOptions Options { get; set; }

        public Pipeline(IEnumerable<IStep> steps, PipelineOptions options = null)
        {
            Steps = steps;
            Options = options ?? new PipelineOptions();
        }
    }
}
