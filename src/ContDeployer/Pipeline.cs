using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace JeremyTCD.ContDeployer
{
    public class Pipeline : IPipeline
    {
        private ILogger<Pipeline> _logger { get; }
        private IPluginFactory _pluginFactory { get; }
        private IPipelineContext _pipelineContext { get; }

        public Pipeline(ILogger<Pipeline> logger,
            IPluginFactory pluginFactory,
            IPipelineContext pipelineContext)
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
                IStep step = _pipelineContext.Steps.First();

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
