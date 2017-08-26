using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.Core;
using Microsoft.Extensions.Logging;
using StructureMap;
using System;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Config
{
    public class ConfigRunner : IConfigRunner
    {
        private IConfigLoader _pipelineLoader { get; }
        private IPipelineContextBuilder _pipelineContextBuilder { get; }
        private ILoggingService<ConfigRunner> _loggingService { get; }
        private IPipelineRunner _pipelineRunner { get; }

        public ConfigRunner(IConfigLoader pipelineLoader, IPipelineContextBuilder pipelineContextBuilder, 
            ILoggingService<ConfigRunner> loggingService, IPipelineRunner pipelineRunner)
        {
            _pipelineContextBuilder = pipelineContextBuilder;
            _pipelineLoader = pipelineLoader;
            _loggingService = loggingService;
            _pipelineRunner = pipelineRunner;
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

            // Create pipeline context
            IPipelineContext pipelineContext = _pipelineContextBuilder.
                AddPipelinesCEOptions(pipelinesCEOptions).
                AddSharedStepOptions(sharedStepOptions).
                AddPluginContainers(pluginContainers).
                BuildPipelineContext();

            // Run
            _loggingService.LogInformation(Strings.Log_RunningPipeline, pipelineContext.PipelinesCEOptions.Pipeline);
            _pipelineRunner.Run(pipeline, pipelineContext);
            _loggingService.LogInformation(Strings.Log_FinishedRunningPipeline, pipelineContext.PipelinesCEOptions.Pipeline);
        }
    }
}
