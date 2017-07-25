﻿using JeremyTCD.PipelinesCE.Core;
using StructureMap;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.ConfigHost
{
    public class ConfigHostCore
    {
        private IPipelineLoader _pipelineLoader { get; }
        private IPipelineRunner _pipelineRunner { get; }
        private IPipelineContextFactory _pipelineContextFactory { get; }

        public ConfigHostCore(IPipelineLoader pipelineLoader, IPipelineRunner pipelineRunner,
            IPipelineContextFactory pipelineContextFactory)
        {
            _pipelineContextFactory = pipelineContextFactory;
            _pipelineLoader = pipelineLoader;
            _pipelineRunner = pipelineRunner;
        }

        public void Run(PipelinesCEOptions pipelinesCEOptions, SharedPluginOptions sharedPluginOptions)
        {
            // Load 
            (Pipeline pipeline, IDictionary<string, IContainer> pluginContainers) = _pipelineLoader.Load(pipelinesCEOptions);

            // Merge options
            pipeline.SharedPluginOptions = sharedPluginOptions.Combine(pipeline.SharedPluginOptions);

            // Create pipeline context
            IPipelineContext pipelineContext = _pipelineContextFactory.
                AddPipelinesCEOptions(pipelinesCEOptions).
                AddSharedPluginOptions(pipeline.SharedPluginOptions).
                AddPluginContainers(pluginContainers).
                CreatePipelineContext();

            // Run
            _pipelineRunner.Run(pipeline, pipelineContext);
        }
    }
}
