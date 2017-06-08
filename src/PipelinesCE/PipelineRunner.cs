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

        public void Run(Pipeline pipeline)
        {
            IPipelineContext pipelineContext = _pipelineContextFactory.
                AddPipelineOptions(pipeline.Options).
                CreatePipelineContext();
            LinkedList<IStep> remainingSteps = new LinkedList<IStep>(pipeline.Steps);

            _logger.LogInformation(Strings.Log_RunningPipeline, pipeline.Options.Pipeline);

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

                _logger.LogInformation(Strings.Log_RunningPlugin, step.PluginType.Name);
                plugin.Run(pipelineContext, stepContext);
                _logger.LogInformation(Strings.Log_PluginComplete, step.PluginType.Name);
            }

            _logger.LogInformation(Strings.Log_PipelineComplete, pipeline.Options.Pipeline);
        }
    }
}
