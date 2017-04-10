using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using JeremyTCD.ContDeployer.PluginTools;

namespace JeremyTCD.ContDeployer
{
    public class Pipeline
    {
        public PipelineContext Context { get; }
        public ILogger<Pipeline> Logger { get; }
        public PipelineOptions Options { get; }
        public ILoggerFactory LoggerFactory { get; }

        public Pipeline(PipelineContext context, IOptions<PipelineOptions> optionsAccessor, 
            ILogger<Pipeline> logger,
            ILoggerFactory loggerFactory)
        {
            Context = context;
            Logger = logger;
            Options = optionsAccessor.Value;
            LoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Runs each pipeline step serially
        /// </summary>
        public void Run()
        {
            // Use linked list since steps will be added to and removed from start of list
            LinkedList<PipelineStep> steps = new LinkedList<PipelineStep>(Options.PipelineSteps);

            Logger.LogInformation("=== Starting pipeline ===");

            while (steps.Count > 0)
            {
                PipelineStep step = steps.First();
                steps.RemoveFirst();

                types.TryGetValue(step.PluginName, out Type pluginType);
                if (pluginType == null)
                {
                    throw new Exception($"{nameof(Pipeline)}: No plugin with name {step.PluginName} exists");
                }

                Logger.LogInformation($"Running step with plugin: {step.PluginName}");
                // TODO avoid creating logger if a logger with the same category already exists?
                ILogger pluginLogger = LoggerFactory.CreateLogger(plugin.GetType().FullName);
                //plugin.Run(step.Config, Context, pluginLogger, steps);
                Logger.LogInformation("Step complete");
            }

            Logger.LogInformation("=== Pipeline complete ===");
        }
    }
}
