using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace JeremyTCD.PipelinesCE
{
    public class PipelineRunner : IPipelineRunner
    {
        private ILogger<PipelineRunner> _logger { get; }
        private IPluginFactory _pluginFactory { get; }
        private IPipelineContextFactory _pipelineContextFactory { get; }
        private IStepContextFactory _stepContextFactory { get; }
        private ILoggerFactory _loggerFactory { get; }

        public PipelineRunner(ILogger<PipelineRunner> logger, 
            IPluginFactory pluginFactory, 
            IStepContextFactory stepContextFactory,
            IPipelineContextFactory pipelineContextFactory,
            ILoggerFactory loggerFactory)
        {
            _stepContextFactory = stepContextFactory;
            _logger = logger;
            _pluginFactory = pluginFactory;
            _loggerFactory = loggerFactory;
            _pipelineContextFactory = pipelineContextFactory;
        }

        public void Run(IEnumerable<IStep> steps, PipelineOptions pipelineOptions)
        {
            IPipelineContext pipelineContext = _pipelineContextFactory.
                AddPipelineOptions(pipelineOptions).
                CreatePipelineContext();
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
                plugin.Run(pipelineContext, stepContext);
                _logger.LogInformation($"== {step.PluginType.Name} complete ==");
            }

            _logger.LogInformation("=== Pipeline complete ===");
        }
    }
}
