using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.Core;
using Microsoft.Extensions.Logging;
using StructureMap;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Config
{
    public class ConfigRunner : IConfigRunner
    {
        private IConfigLoader _pipelineLoader { get; }
        private IStepGraphFactory _stepGraphFactory { get; }
        private IPipelineContextBuilder _pipelineContextBuilder { get; }
        private ILoggerFactory _loggerFactory { get; }
        private ILoggingService<ConfigRunner> _loggingService { get; }

        public ConfigRunner(IConfigLoader pipelineLoader, IStepGraphFactory stepGraphFactory, ILoggerFactory loggerFactory,
            IPipelineContextBuilder pipelineContextBuilder, ILoggingService<ConfigRunner> loggingService)
        {
            _pipelineContextBuilder = pipelineContextBuilder;
            _pipelineLoader = pipelineLoader;
            _stepGraphFactory = stepGraphFactory;
            _loggerFactory = loggerFactory;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Loads <see cref="Pipeline"/>, creates <see cref="PipelineContext"/> and runs <see cref="Pipeline"/>
        /// </summary>
        /// <param name="pipelinesCEOptions"></param>
        /// <param name="sharedStepOptions"></param>
        public void Run(PipelinesCEOptions pipelinesCEOptions, SharedStepOptions sharedStepOptions)
        {
            // Load Pipeline
            (Pipeline pipeline, IDictionary<string, IContainer> pluginContainers) = _pipelineLoader.Load(pipelinesCEOptions);

            // Create StepGraph
            StepGraph stepGraph = _stepGraphFactory.CreateFromComposableGroup(pipeline);

            // Create pipeline context
            IPipelineContext pipelineContext = _pipelineContextBuilder.
                AddPipelinesCEOptions(pipelinesCEOptions).
                AddSharedStepOptions(sharedStepOptions).
                AddPluginContainers(pluginContainers).
                AddLoggerFactory(_loggerFactory).
                BuildPipelineContext();

            // Run
            _loggingService.LogInformation(Strings.Log_RunningPipeline, pipelineContext.PipelinesCEOptions.Pipeline);
            stepGraph.Run(pipelineContext);
            _loggingService.LogInformation(Strings.Log_FinishedRunningPipeline, pipelineContext.PipelinesCEOptions.Pipeline);
        }
    }
}
