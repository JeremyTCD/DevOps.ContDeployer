using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using StructureMap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace JeremyTCD.PipelinesCE.ConfigHost
{
    public class PipelineLoader : IPipelineLoader, IDisposable
    {
        private IAssemblyService _assemblyService { get; }
        private ILoggingService<PipelineLoader> _loggingService { get; }
        private IPathService _pathService { get; }
        private IDirectoryService _directoryService { get; }
        private IMSBuildService _msBuildService { get; }
        private IActivatorService _activatorService { get; }
        private IContainer _mainContainer { get; }
        private IDependencyContextService _dependencyContextService { get; }

        private IDictionary<string, IContainer> _pluginContainers { get; set; }
        private bool _isDisposed;

        public bool IsDisposed
        {
            get
            {
                return _isDisposed;
            }
        }

        public PipelineLoader(IActivatorService activatorService,
            IDependencyContextService dependencyContextService,
            IAssemblyService assemblyService,
            IPathService pathService,
            IDirectoryService directoryService,
            IMSBuildService msBuildService,
            IContainer mainContainer,
            ILoggingService<PipelineLoader> loggingService)
        {
            _dependencyContextService = dependencyContextService;
            _mainContainer = mainContainer;
            _msBuildService = msBuildService;
            _pathService = pathService;
            _assemblyService = assemblyService;
            _directoryService = directoryService;
            _loggingService = loggingService;
            _activatorService = activatorService;
        }

        /// <summary>
        /// Loads <see cref="Pipeline"/> specified by <see cref="PipelineOptions.Pipeline"/>
        /// </summary>
        /// <param name="pipelineOptions"></param>
        /// <returns></returns>
        public virtual (Pipeline, IDictionary<string, IContainer>) Load(PipelineOptions pipelineOptions)
        {
            _loggingService.LogInformation(Strings.Log_LoadingPipeline);

            // Load assemblies
            _loggingService.LogDebug(Strings.Log_LoadingAssemblies);
            IEnumerable<Assembly> assemblies = LoadAssemblies();
            _loggingService.LogDebug(Strings.Log_SuccessfullyLoadedAssemblies);

            // Create plugin IoC containers
            _loggingService.LogDebug(Strings.Log_CreatingPluginIoCContainers);
            _pluginContainers = CreatePluginIoCContainers(assemblies);
            _loggingService.LogDebug(Strings.Log_SuccessfullyCreatedPluginContainers);

            // Create pipeline factory
            _loggingService.LogInformation(Strings.Log_CreatingPipeline, pipelineOptions.Pipeline);
            IPipelineFactory factory = CreatePipelineFactory(assemblies, pipelineOptions);
            _loggingService.LogInformation(Strings.Log_SuccessfullyCreatedPipeline, pipelineOptions.Pipeline);

            _loggingService.LogInformation(Strings.Log_SuccessfullyLoadedPipeline);

            // Create pipeline
            Pipeline pipeline = factory.CreatePipeline();

            return (pipeline, _pluginContainers);
        }

        // TODO Access modifier should be internal or private but no good way to test if so
        /// <summary>
        /// Loads assemblies that reference JeremyTCD.PipelinesCE.Tools from the directory that the executing assembly is located in
        /// </summary>
        /// <returns>
        /// <see cref="IEnumerable{Assembly}"/>
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if more than 1 *.deps.json file exists in directory
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if no *.deps.json file exists in directory
        /// </exception>
        public virtual IEnumerable<Assembly> LoadAssemblies()
        {
            string directory = _directoryService.GetParent(typeof(PipelineLoader).GetTypeInfo().Assembly.Location).FullName;

            string[] possibleDepsFiles = _directoryService.GetFiles(directory, "*.deps.json", SearchOption.TopDirectoryOnly);
            if (possibleDepsFiles.Length > 1)
            {
                throw new InvalidOperationException(String.Format(Strings.Exception_MultipleDepsFiles, directory));
            }
            else if (possibleDepsFiles.Length == 0)
            {
                throw new InvalidOperationException(String.Format(Strings.Exception_NoDepsFiles, directory));
            }
            string depsFile = possibleDepsFiles[0];

            // Create context 
            DependencyContext context = _dependencyContextService.CreateDependencyContext(depsFile);

            return _assemblyService.CreateReferencingAssemblies(context, typeof(IPlugin).GetTypeInfo().Assembly);
        }

        // TODO Access modifier should be internal or private but no good way to test if so
        /// <summary>
        /// Creates an IOC container for each <see cref="IPlugin"/> implementation in <paramref name="assemblies"/>. Populates containers 
        /// using corresponding <see cref="IPluginStartup"/> implementations.
        /// </summary>
        /// <param name="assemblies"></param>
        public virtual IDictionary<string, IContainer> CreatePluginIoCContainers(IEnumerable<Assembly> assemblies)
        {
            IDictionary<string, IContainer> result = new Dictionary<string, IContainer>();

            List<Type> pluginTypes = _assemblyService.GetAssignableTypes(assemblies, typeof(IPlugin)).ToList();
            IDictionary<string, Type> pluginStartupTypes = _assemblyService.GetAssignableTypes(assemblies, typeof(IPluginStartup)).ToDictionary(t => t.Name);
            foreach (Type pluginType in pluginTypes)
            {
                // Configure services
                _loggingService.LogDebug(Strings.Log_CreatingPluginIoCContainer, pluginType.Name);
                ServiceCollection services = new ServiceCollection();
                pluginStartupTypes.TryGetValue($"{pluginType.Name}Startup", out Type pluginStartupType);
                if (pluginStartupType != null)
                {
                    IPluginStartup pluginStartup = (IPluginStartup)_activatorService.CreateInstance(pluginStartupType);
                    _loggingService.LogDebug(Strings.Log_ConfiguringPluginServices, pluginType.Name, pluginStartupType.Name);
                    pluginStartup.ConfigureServices(services);
                }
                IContainer pluginContainer = _mainContainer.CreateChildContainer();
                pluginContainer.Configure(registry =>
                {
                    ((Registry)registry).Populate(services);
                    registry.For(pluginType).Use(pluginType).Singleton();
                });
                result.Add(pluginType.Name, pluginContainer);
            }

            return result;
        }

        // TODO Access modifier should be internal or private but no good way to test if so
        /// <summary>
        /// Gets <see cref="IPipelineFactory"/> from project assemblies that creates a pipeline with name 
        /// <paramref name="pipelineOptions"/>. If <paramref name="pipelineOptions"/> is null and there is only one <see cref="IPipelineFactory"/> 
        /// implementation, returns an instance of the sole implementation.
        /// </summary>
        /// <param name="projectFile"></param>
        /// <param name="pipelineOptions"></param>
        /// <returns>
        /// <see cref="IPipelineFactory"/>
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if project does not contain any <see cref="IPipelineFactory"/> implementations
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <paramref name="pipelineOptions"/> is null and there are multiple pipeline factories
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if no <see cref="IPipelineFactory"/> produces a pipeline with name <paramref name="pipelineOptions"/>
        /// </exception>
        public virtual IPipelineFactory CreatePipelineFactory(IEnumerable<Assembly> assemblies, PipelineOptions pipelineOptions)
        {
            _loggingService.LogDebug(Strings.Log_CreatingPipelineFactory, pipelineOptions.Pipeline);

            IEnumerable<Type> pipelineFactoryTypes = _assemblyService.GetAssignableTypes(assemblies, typeof(IPipelineFactory));

            if (pipelineFactoryTypes.Count() == 0)
            {
                throw new InvalidOperationException(Strings.Exception_NoPipelineFactories);
            }

            Type pipelineFactoryType;
            if (pipelineOptions.Pipeline == PipelineOptions.DefaultPipeline)
            {
                if (pipelineFactoryTypes.Count() == 1)
                {
                    pipelineFactoryType = pipelineFactoryTypes.First();
                    pipelineOptions.Pipeline = PipelineFactoryPipelineName(pipelineFactoryType); // Default pipeline resolved
                    _loggingService.LogInformation(Strings.Log_ResolvedDefaultPipeline, pipelineOptions.Pipeline);
                }
                else
                {
                    throw new InvalidOperationException(string.Format(Strings.Exception_MultiplePipelineFactories,
                        string.Join(Environment.NewLine, pipelineFactoryTypes.Select(t => t.Name))));
                }
            }
            else
            {
                IEnumerable<Type> types = pipelineFactoryTypes.
                    Where(t => string.Equals(PipelineFactoryPipelineName(t), pipelineOptions.Pipeline, StringComparison.OrdinalIgnoreCase));
                if (types.Count() == 0)
                {
                    throw new InvalidOperationException(string.Format(Strings.Exception_NoPipelineFactory, pipelineOptions.Pipeline));
                }
                if (types.Count() > 1)
                {
                    throw new InvalidOperationException(string.Format(Strings.Exception_MultiplePipelineFactoriesWithSameName,
                        pipelineOptions.Pipeline,
                        string.Join(Environment.NewLine, pipelineFactoryTypes.Select(t => t.FullName))));
                }
                pipelineFactoryType = types.First();
            }

            return (IPipelineFactory)_activatorService.CreateInstance(pipelineFactoryType);
        }

        // TODO Access modifier should be internal or private but no good way to test if so
        public virtual string PipelineFactoryPipelineName(Type pipelineFactoryType)
        {
            return pipelineFactoryType.Name.Replace("PipelineFactory", "");
        }

        public void Dispose()
        {
            if (!_isDisposed && _pluginContainers != null)
            {
                foreach (IContainer container in _pluginContainers.Values)
                {
                    try
                    {
                        container.Dispose();
                    }
                    catch (Exception exception)
                    {
                        // Log but swallow exception so that all containers are disposed of
                        _loggingService.LogError(exception.ToString());
                    }
                }
            }
            _isDisposed = true;
        }
    }
}
