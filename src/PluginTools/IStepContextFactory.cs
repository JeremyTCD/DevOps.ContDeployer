using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public interface IStepContextFactory
    {
        IStepContextFactory AddRemainingSteps(IEnumerable<IStep> steps);
        IStepContextFactory AddPluginOptions(IPluginOptions options);
        IStepContextFactory AddLogger(ILogger logger);

        IStepContext Build();
    }
}
