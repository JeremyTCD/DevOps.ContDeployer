using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;
using System.Linq;
using JeremyTCD.ContDeployer.PluginTools;

namespace JeremyTCD.ContDeployer
{
    public class Pipeline
    {
        public PipelineContext Context { get; }
        public ILogger<Pipeline> Logger { get; }
        public PipelineOptions Options { get; }

        public Pipeline(PipelineContext context, IOptions<PipelineOptions> optionsAccessor, 
            ILogger<Pipeline> logger)
        {
            Context = context;
            Logger = logger;
            Options = optionsAccessor.Value;
        }

        /// <summary>
        /// Executes each step serially
        /// </summary>
        public void Run()
        {
            // Use linked list since steps will be added to and removed from start of list
            LinkedList<PipelineStep> steps = new LinkedList<PipelineStep>(Options.PipelineSteps);

            Logger.LogInformation("=== Starting pipeline ===");

            // use linked list
            while (steps.Count > 0)
            {
                PipelineStep step = steps.First();
                steps.RemoveFirst();

                IPlugin plugin;
                if (!Context.Plugins.TryGetValue(step.PluginName, out plugin))
                {
                    throw new Exception($"{nameof(Pipeline)}: No plugin with name {step.PluginName} exists");
                }

                Logger.LogInformation($"Running step with plugin: {step.PluginName}");
                plugin.Execute(step.Config, Context, steps);
                Logger.LogInformation("Step complete");
            }

            Logger.LogInformation("=== Pipeline complete ===");
        }
    }
}
