using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace JeremyTCD.PipelinesCE.PluginTools
{
    public class StepContext : IStepContext
    {
        public IPluginOptions PluginOptions { get; set; }
        public ILogger Logger { get; set; }
        public LinkedList<IStep> RemainingSteps { get; set; }
    }
}
