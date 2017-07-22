using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Tools
{
    /// <summary>
    /// Factory for <see cref="IStepContext"/> instances. This interface decouples <see cref="IStepContext"/> from <see cref="StepContext"/>.
    /// Uses multiple functions to specify objects required to build instead of multiple parameters on the <see cref="CreateStepContext"/> function so that 
    /// objects can be added or removed without changing <see cref="CreateStepContext"/>. 
    /// </summary>
    public interface IStepContextFactory
    {
        IStepContextFactory AddRemainingSteps(LinkedList<IStep> remainingSteps);
        IStepContextFactory AddPluginOptions(IPluginOptions pluginOptions);
        IStepContextFactory AddLogger(ILogger logger);

        IStepContext CreateStepContext();
    }
}
