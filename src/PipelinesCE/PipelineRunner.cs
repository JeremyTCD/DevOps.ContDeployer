using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace JeremyTCD.PipelinesCE
{
    public class PipelineRunner : IPipelineRunner
    {
        private ILoggingService<PipelineRunner> _loggingService { get; }
        private IPluginFactory _pluginFactory { get; }
        private IPipelineContextFactory _pipelineContextFactory { get; }
        private IStepContextFactory _stepContextFactory { get; }
        private ILoggerFactory _loggerFactory { get; }

        public PipelineRunner(ILoggingService<PipelineRunner> loggingService, 
            IPluginFactory pluginFactory, 
            IStepContextFactory stepContextFactory,
            IPipelineContextFactory pipelineContextFactory,
            ILoggerFactory loggerFactory)
        {
            _stepContextFactory = stepContextFactory;
            _loggingService = loggingService;
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

            _loggingService.LogInformation(Strings.Log_RunningPipeline, pipeline.Options.Pipeline);

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

                _loggingService.LogInformation(Strings.Log_RunningPlugin, step.PluginType.Name);
                plugin.Run(pipelineContext, stepContext);
                _loggingService.LogInformation(Strings.Log_PluginComplete, step.PluginType.Name);
            }

            _loggingService.LogInformation(Strings.Log_PipelineComplete, pipeline.Options.Pipeline);
        }
    }
}
