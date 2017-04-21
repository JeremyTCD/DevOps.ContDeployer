using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JeremyTCD.ContDeployer
{
    public class StepContextFactory
    {
        private IAssemblyService _assemblyService { get; }
        private ILogger<StepContextFactory> _logger { get; }
        private ILoggerFactory _loggerFactory { get; }
        private PipelineContext _pipelineContext { get; }

        private Dictionary<string, Type> _pluginOptionsTypes { get; set; }
        private Dictionary<string, string> _pluginTypeFullNames { get; set; }

        public StepContextFactory(IAssemblyService assemblyService,
            ILoggerFactory loggerFactory,
            ILogger<StepContextFactory> logger,
            PipelineContext piplineContext)
        {
            _assemblyService = assemblyService;
            _logger = logger;
            _loggerFactory = loggerFactory;
            _pipelineContext = piplineContext;
        }

        public void LoadTypes()
        {
            IEnumerable<Assembly> pluginAssemblies = _assemblyService.GetReferencingAssemblies(typeof(IPlugin).GetTypeInfo().Assembly);

            _pluginOptionsTypes = _assemblyService.GetAssignableTypes(pluginAssemblies, typeof(IPluginOptions)).
                ToDictionary(type => type.Name);

            _pluginTypeFullNames = _assemblyService.GetAssignableTypes(pluginAssemblies, typeof(IPlugin)).
                ToDictionary(type => type.Name, type => type.FullName);
        }

        public StepContext Build()
        {
            Step step = _pipelineContext.Steps.First();
            
            // Plugin options
            IPluginOptions pluginOptions = null;
            _pluginOptionsTypes.TryGetValue($"{step.PluginName}Options", out Type pluginOptionsType);
            if (pluginOptionsType == null)
            {
                _logger.LogInformation($"No options type for plugin with name \"{step.PluginName}\"");
            }
            else
            {
                if (step.Options != null && step.Options.GetType().Equals(pluginOptionsType))
                {
                    pluginOptions = step.Options;
                }
                else
                {
                    pluginOptions = Activator.CreateInstance(pluginOptionsType) as IPluginOptions;

                    if (step.Config != null && !string.IsNullOrEmpty(step.Config.Value))
                    {
                        step.Config.Bind(pluginOptions);
                    }
                }

                pluginOptions.Validate();
                _logger.LogInformation($"Plugin options for plugin with name \"{step.PluginName}\" successfully built");
            }

            ILogger logger = _loggerFactory.CreateLogger(_pluginTypeFullNames[step.PluginName]);

            return new StepContext
            {
                Options = pluginOptions,
                Logger = logger
            };
        }
    }
}
