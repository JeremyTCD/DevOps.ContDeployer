using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace JeremyTCD.ContDeployer
{
    public class PipelineStepContextFactory
    {
        private IAssemblyService _assemblyService { get; }
        private ILogger<PipelineStepContextFactory> _logger { get; }
        private ILoggerFactory _loggerFactory { get; }
        private Dictionary<string, Type> _pluginOptionsTypes { get; set; }
        private PipelineStep _step { get; set; }

        public PipelineStepContextFactory(IAssemblyService assemblyService,
            ILoggerFactory loggerFactory,
            ILogger<PipelineStepContextFactory> logger)
        {
            _assemblyService = assemblyService;
            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        public void LoadTypes()
        {
            AssemblyService assemblyService = new AssemblyService();
            assemblyService.LoadAssembliesInDir(Path.Combine(Directory.GetCurrentDirectory(), "plugins"), true);
            IEnumerable<Assembly> pluginIOptionsAssemblies = assemblyService.GetReferencingAssemblies(typeof(IPluginOptions).GetTypeInfo().Assembly);

            _pluginOptionsTypes = assemblyService.GetAssignableTypes(pluginIOptionsAssemblies, typeof(IPluginOptions)).
                ToDictionary(type => type.Name);
        }

        public PipelineStepContextFactory AddPipelineStep(PipelineStep step)
        {
            _step = step;

            return this;
        }

        public PipelineStepContext Build()
        {
            _pluginOptionsTypes.TryGetValue(_step.PluginName, out Type pluginOptionsType);
            IPluginOptions pluginOptions = null;

            if (pluginOptionsType == null)
            {
                _logger.LogInformation($"No options type for plugin with name \"{_step.PluginName}\"");
            }
            else
            {
                pluginOptions = Activator.CreateInstance(pluginOptionsType) as IPluginOptions;

                _logger.LogInformation($"Plugin options for plugin with name \"{_step.PluginName}\" successfully built");
            }

            ILogger logger = _loggerFactory.CreateLogger(pluginOptionsType.FullName);

            PipelineStepContext pipelineStepContext = new PipelineStepContext
            {
                Options = pluginOptions,
                Logger = logger
            };

            return pipelineStepContext;
        }
    }
}
