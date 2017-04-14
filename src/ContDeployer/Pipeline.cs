using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;

namespace JeremyTCD.ContDeployer
{
    public class Pipeline
    {
        public ILogger<Pipeline> Logger { get; }
        public PipelineOptions Options { get; }
        public IPluginFactory PluginFactory { get; }
        public PipelineContextFactory PipelineContextFactory { get; }
        public PipelineStepContextFactory PipelineStepContextFactory { get; }

        public Pipeline(IOptions<PipelineOptions> optionsAccessor,
            ILogger<Pipeline> logger,
            IPluginFactory pluginFactory,
            PipelineContextFactory pipelineContextFactory,
            PipelineStepContextFactory pipelineStepContextFactory)
        {
            Logger = logger;
            Options = optionsAccessor.Value;
            PluginFactory = pluginFactory;
            PipelineContextFactory = pipelineContextFactory;
            PipelineStepContextFactory = pipelineStepContextFactory;
        }

        /// <summary>
        /// Runs each pipeline step serially
        /// </summary>
        public void Run()
        {
            PipelineContext pipelineContext = PipelineContextFactory.
                AddPipelineSteps(Options.PipelineSteps).
                Build();

            Logger.LogInformation("=== Running pipeline ===");

            while (pipelineContext.PipelineSteps.Count > 0)
            {
                PipelineStep step = pipelineContext.PipelineSteps.First();

                IPlugin plugin = PluginFactory.
                    SetPluginName(step.PluginName).
                    Build();

                PipelineStepContext pipelineStepContext = PipelineStepContextFactory.
                    AddPipelineStep(step).
                    Build();

                pipelineContext.PipelineSteps.RemoveFirst();
                Logger.LogInformation($"== Running {plugin.GetType().Name} ==");
                plugin.Run(pipelineContext, pipelineStepContext);
                Logger.LogInformation($"== {plugin.GetType().Name} complete ==");
            }

            Logger.LogInformation("=== Pipeline complete ===");
        }
    }
}
