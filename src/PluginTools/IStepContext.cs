using Microsoft.Extensions.Logging;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public interface IStepContext
    {
        IPluginOptions Options { get; set; }
        ILogger Logger { get; set; }
    }
}
