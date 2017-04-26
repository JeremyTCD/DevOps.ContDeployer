using System;
using System.Collections.Generic;
using System.Linq;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public class PipelineContextOptions
    {
        public virtual List<IStep> Steps { get; set; } = new List<IStep>();

        public void Validate()
        {
            if(Steps == null || !Steps.Any()) {
                throw new Exception($"{nameof(PipelineContextOptions)}: Pipeline steps required");
            }

            IStep[] steps = Steps.ToArray();
            foreach(IStep step in steps)
            {
                if (String.IsNullOrEmpty(step.PluginName))
                {
                    throw new Exception($"Pipeline step must have plugin name");
                }
            }
        }
    }
}
