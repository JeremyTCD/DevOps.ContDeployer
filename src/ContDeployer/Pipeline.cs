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
        public StepContextFactory StepContextFactory { get; }

        public Pipeline(IOptions<PipelineOptions> optionsAccessor,
            ILogger<Pipeline> logger,
            IPluginFactory pluginFactory,
            PipelineContextFactory pipelineContextFactory,
            StepContextFactory stepContextFactory)
        {
            Logger = logger;
            Options = optionsAccessor.Value;
            PluginFactory = pluginFactory;
            PipelineContextFactory = pipelineContextFactory;
            StepContextFactory = stepContextFactory;
        }

        /// <summary>
        /// Runs each pipeline step serially
        /// </summary>
        public void Run()
        {
            PipelineContext pipelineContext = PipelineContextFactory.
                AddSteps(Options.Steps).
                Build();

            Logger.LogInformation("=== Running pipeline ===");

            while (pipelineContext.Steps.Count > 0)
            {
                Step step = pipelineContext.Steps.First();

                IPlugin plugin = PluginFactory.
                    SetPluginName(step.PluginName).
                    Build();

                StepContext stepContext = StepContextFactory.
                    AddStep(step, plugin.GetType().FullName).
                    Build();

                pipelineContext.Steps.RemoveFirst();
                Logger.LogInformation($"== Running {plugin.GetType().Name} ==");
                plugin.Run();
                Logger.LogInformation($"== {plugin.GetType().Name} complete ==");
            }

            Logger.LogInformation("=== Pipeline complete ===");
        }
    }
}
