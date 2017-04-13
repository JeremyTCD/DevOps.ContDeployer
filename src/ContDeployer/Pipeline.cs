﻿using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace JeremyTCD.ContDeployer
{
    public class Pipeline
    {
        public ILogger<Pipeline> Logger { get; }
        public PipelineOptions Options { get; }
        public IPluginFactory PluginFactory { get; }

        public Pipeline(IOptions<PipelineOptions> optionsAccessor,
            ILogger<Pipeline> logger,
            IPluginFactory pluginFactory)
        {
            Logger = logger;
            Options = optionsAccessor.Value;
            PluginFactory = pluginFactory;
        }

        /// <summary>
        /// Runs each pipeline step serially
        /// </summary>
        public void Run()
        {
            // Use linked list since steps will be added to and removed from start of list
            LinkedList<PipelineStep> steps = new LinkedList<PipelineStep>(Options.PipelineSteps);
            Dictionary<string, object> sharedData = new Dictionary<string, object>();

            Logger.LogInformation("=== Running pipeline ===");

            while (steps.Count > 0)
            {
                PipelineStep step = steps.First();

                IPlugin plugin = PluginFactory.BuildPluginForPipelineStep(step);

                steps.RemoveFirst();
                Logger.LogInformation($"== Running {plugin.GetType().Name} ==");
                plugin.Run(sharedData, steps);
                Logger.LogInformation($"== {plugin.GetType().Name} complete ==");
            }

            Logger.LogInformation("=== Pipeline complete ===");
        }
    }
}
