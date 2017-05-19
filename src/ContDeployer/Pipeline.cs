using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer
{
    public class Pipeline : IPipeline
    {
        private ILogger<Pipeline> _logger { get; }
        private IPluginFactory _pluginFactory { get; }
        private IPipelineContext _pipelineContext { get; }
        private IStepContextFactory _stepContextFactory { get; }
        private ILoggerFactory _loggerFactory { get; }

        public LinkedList<IStep> Steps { get; set; }
        public string Name { get; set; }

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
        public void Run()
        {
            _logger.LogInformation("=== Running pipeline ===");

            while (Steps.Count > 0)
            {
                IStep step = Steps.First();
                Steps.RemoveFirst();

                IPlugin plugin = _pluginFactory.
                    Build(step.PluginType);
                ILogger logger = _loggerFactory.CreateLogger(step.PluginType.Name);
                IStepContext stepContext = _stepContextFactory.
                    AddPluginOptions(step.PluginOptions).
                    AddRemainingSteps(Steps).
                    AddLogger(logger).
                    Build();

                _logger.LogInformation($"== Running {step.PluginType.Name} ==");
                plugin.Run(_pipelineContext, stepContext);
                _logger.LogInformation($"== {step.PluginType.Name} complete ==");
            }

            _logger.LogInformation("=== Pipeline complete ===");
        }
    }
}
