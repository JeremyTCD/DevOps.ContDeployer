using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public interface IStepContext
    {
        IPluginOptions Options { get; set; }
        ILogger Logger { get; set; }
        IEnumerable<IStep> RemainingSteps { get; set; }
    }
}
