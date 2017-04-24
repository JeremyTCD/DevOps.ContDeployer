using Microsoft.Extensions.Logging;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public class StepContext : IStepContext
    {
        public IPluginOptions Options { get; set; }
        public ILogger Logger { get; set; }
    }
}
