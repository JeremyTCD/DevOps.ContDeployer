using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace JeremyTCD.PipelinesCE
{
    public class Pipeline : IPipeline
    {
        private ILogger<Pipeline> _logger { get; }
        private IPluginFactory _pluginFactory { get; }
        private IPipelineContext _pipelineContext { get; }
        private IStepContextFactory _stepContextFactory { get; }
        private ILoggerFactory _loggerFactory { get; }

        public Pipeline(ILogger<Pipeline> logger, IPluginFactory pluginFactory, 
            IPipelineContext pipelineContext, IStepContextFactory stepContextFactory,
            ILoggerFactory loggerFactory)
        {
            _stepContextFactory = stepContextFactory;
            _pipelineContext = pipelineContext;
            _logger = logger;
            _pluginFactory = pluginFactory;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Runs each pipeline step serially
        /// </summary>
        public void Run(IEnumerable<IStep> steps)
        {
            LinkedList<IStep> remainingSteps = new LinkedList<IStep>(steps);

            _logger.LogInformation("=== Running pipeline ===");

            while (remainingSteps.Count > 0)
            {
                IStep step = remainingSteps.First();
                remainingSteps.RemoveFirst();

                IPlugin plugin = _pluginFactory.CreatePlugin(step.PluginType);
                ILogger logger = _loggerFactory.CreateLogger(step.PluginType.Name);
                IStepContext stepContext = _stepContextFactory.
                    AddPluginOptions(step.PluginOptions).
                    AddRemainingSteps(remainingSteps).
                    AddLogger(logger).
                    CreateStepContext();

                _logger.LogInformation($"== Running {step.PluginType.Name} ==");
                plugin.Run(_pipelineContext, stepContext);
                _logger.LogInformation($"== {step.PluginType.Name} complete ==");
            }

            _logger.LogInformation("=== Pipeline complete ===");
        }
    }
}
