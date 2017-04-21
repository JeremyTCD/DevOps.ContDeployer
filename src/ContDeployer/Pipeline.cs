using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;

namespace JeremyTCD.ContDeployer
{
    public class Pipeline
    {
        private ILogger<Pipeline> _logger { get; }
        private IPluginFactory _pluginFactory { get; }
        private PipelineContext _pipelineContext { get; }

        public Pipeline(ILogger<Pipeline> logger,
            IPluginFactory pluginFactory,
            PipelineContext pipelineContext)
        {
            _pipelineContext = pipelineContext;
            _logger = logger;
            _pluginFactory = pluginFactory;
        }

        /// <summary>
        /// Runs each pipeline step serially
        /// </summary>
        public void Run()
        {
            _logger.LogInformation("=== Running pipeline ===");

            while (_pipelineContext.Steps.Count > 0)
            {
                Step step = _pipelineContext.Steps.First();

                IPlugin plugin = _pluginFactory.
                    SetPluginName(step.PluginName).
                    Build();

                _pipelineContext.Steps.RemoveFirst();
                _logger.LogInformation($"== Running {plugin.GetType().Name} ==");
                plugin.Run();
                _logger.LogInformation($"== {plugin.GetType().Name} complete ==");
            }

            _logger.LogInformation("=== Pipeline complete ===");
        }
    }
}
