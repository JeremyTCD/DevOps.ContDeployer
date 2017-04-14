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
    public class StepContextFactory
    {
        private IAssemblyService _assemblyService { get; }
        private ILogger<StepContextFactory> _logger { get; }
        private ILoggerFactory _loggerFactory { get; }
        private Dictionary<string, Type> _pluginOptionsTypes { get; set; }
        private Step _step { get; set; }

        public StepContextFactory(IAssemblyService assemblyService,
            ILoggerFactory loggerFactory,
            ILogger<StepContextFactory> logger)
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

        public StepContextFactory AddStep(Step step)
        {
            _step = step;

            return this;
        }

        public StepContext Build()
        {
            _pluginOptionsTypes.TryGetValue($"{_step.PluginName}Options", out Type pluginOptionsType);
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

            ILogger logger = _loggerFactory.CreateLogger(_step.PluginName);

            StepContext stepContext = new StepContext
            {
                Options = pluginOptions,
                Logger = logger
            };

            return stepContext;
        }
    }
}
