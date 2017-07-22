using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Tools
{
    public class StepContext : IStepContext
    {
        public IPluginOptions PluginOptions { get; set; }
        public ILogger Logger { get; set; }
        public LinkedList<IStep> RemainingSteps { get; set; }
    }
}
