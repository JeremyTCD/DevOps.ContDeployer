using JeremyTCD.ContDeployer.PluginTools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JeremyTCD.ContDeployer
{
    public class PipelineOptions
    {
        public List<PipelineStep> PipelineSteps { get; set; } = new List<PipelineStep>();

        public void Validate()
        {
            if(PipelineSteps == null || !PipelineSteps.Any()) {
                throw new Exception($"{nameof(PipelineOptions)}: Pipeline steps required");
            }

            PipelineStep[] pipelineSteps = PipelineSteps.ToArray();
            foreach(PipelineStep step in pipelineSteps)
            {
                if (String.IsNullOrEmpty(step.PluginName))
                {
                    throw new Exception($"Pipeline step must have plugin name");
                }
            }
        }
    }
}
