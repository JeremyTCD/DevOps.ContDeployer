using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.PluginTools
{
    public class StepContextFactory : IStepContextFactory
    {
        private ILogger _logger { get; set; }
        private IPluginOptions _pluginOptions { get; set; }
        private LinkedList<IStep> _remainingSteps { get; set; }

        public IStepContextFactory AddLogger(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        public IStepContextFactory AddPluginOptions(IPluginOptions pluginOptions)
        {
            _pluginOptions = pluginOptions;
            return this;
        }

        public IStepContextFactory AddRemainingSteps(LinkedList<IStep> remainingSteps)
        {
            _remainingSteps = remainingSteps;
            return this;
        }

        public IStepContext CreateStepContext()
        {
            return new StepContext
            {
                Logger = _logger,
                PluginOptions = _pluginOptions,
                RemainingSteps = _remainingSteps
            };
        }
    }
}
