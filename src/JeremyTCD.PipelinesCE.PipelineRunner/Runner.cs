using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using Microsoft.Extensions.Logging;
using StructureMap;
using System.Collections.Generic;
using System.Linq;

namespace JeremyTCD.PipelinesCE.PipelineRunner
{
    public class Runner : IRunner
    {
        private ILoggingService<Runner> _loggingService { get; }
        private IPipelineContextFactory _pipelineContextFactory { get; }
        private IStepContextFactory _stepContextFactory { get; }
        private ILoggerFactory _loggerFactory { get; }

        public Runner(ILoggingService<Runner> loggingService,
            IStepContextFactory stepContextFactory,
            IPipelineContextFactory pipelineContextFactory,
            ILoggerFactory loggerFactory)
        {
            _stepContextFactory = stepContextFactory;
            _loggingService = loggingService;
            _loggerFactory = loggerFactory;
            _pipelineContextFactory = pipelineContextFactory;
        }

        public void Run(Pipeline pipeline, IDictionary<string, IContainer> pluginContainers)
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

                IContainer container = pluginContainers[step.PluginType.Name];
                IPlugin plugin = container.GetInstance(step.PluginType) as IPlugin;
                ILogger logger = _loggerFactory.CreateLogger(step.PluginType.Name);

                IStepContext stepContext = _stepContextFactory.
                    AddPluginOptions(step.PluginOptions).
                    AddRemainingSteps(remainingSteps).
                    AddLogger(logger).
                    CreateStepContext();

                _loggingService.LogInformation(Strings.Log_RunningPlugin, step.PluginType.Name);
                plugin.Run(pipelineContext, stepContext);
                _loggingService.LogInformation(Strings.Log_FinishedRunningPlugin, step.PluginType.Name);
            }

            _loggingService.LogInformation(Strings.Log_FinishedRunningPipeline, pipeline.Options.Pipeline);
        }
    }
}
