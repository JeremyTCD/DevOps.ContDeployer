using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;

namespace JeremyTCD.DevOps.ContDeployer
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

        public void Run()
        {
            Logger.LogInformation("=== Starting pipeline ===");

            // use linked list
            int i = 0;
            while (i < Options.PipelineSteps.Count)
            {
                PipelineStep pipelineStep = Options.PipelineSteps[i];

                IPlugin plugin;
                if (!Context.Plugins.TryGetValue(pipelineStep.PluginName, out plugin))
                {
                    throw new Exception($"{nameof(Pipeline)}: No plugin with name {pipelineStep.PluginName} exists");
                }

                plugin.Execute(/*metadata, config, allconfigs*/);

                i++;
            }

            Logger.LogInformation("=== Pipeline complete ===");
        }
    }
}
