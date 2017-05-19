﻿using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.PluginTools
{
    /// <summary>
    /// Factory for <see cref="IStepContext"/> instances. This interface decouples <see cref="IStepContext"/> from <see cref="StepContext"/>.
    /// Uses multiple functions to specify objects required to build instead of multiple parameters on the <see cref="Build"/> function so that 
    /// objects can be added or removed without changing <see cref="Build"/>. 
    /// </summary>
    public interface IStepContextFactory
    {
        IStepContextFactory AddRemainingSteps(IEnumerable<IStep> steps);
        IStepContextFactory AddPluginOptions(IPluginOptions options);
        IStepContextFactory AddLogger(ILogger logger);

        IStepContext Build();
    }
}
