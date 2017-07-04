using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using NuGet.Configuration;
using StructureMap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace JeremyTCD.PipelinesCE
{
    public class PipelinesCE : IDisposable
    {
        private IAssemblyService _assemblyService { get; }
        private IPipelineRunner _pipelineRunner { get; }
        private ILoggingService<PipelinesCE> _loggingService { get; }
        private IPathService _pathService { get; }
        private IDirectoryService _directoryService { get; }
        private IMSBuildService _msBuildService { get; }
        private IActivatorService _activatorService { get; }
        private IContainer _mainContainer { get; }
        private IDependencyContextService _dependencyContextService { get; }
        private INugetConfigurationService _nugetConfigurationService { get; }

        private IDictionary<string, IContainer> _pluginContainers { get; set; }
        private bool _isDisposed;

        public bool IsDisposed
        {
            get
            {
                return _isDisposed;
            }
        }

        public PipelinesCE(IActivatorService activatorService,
            IDependencyContextService dependencyContextService,
            IAssemblyService assemblyService,
            IPathService pathService,
            IDirectoryService directoryService,
            IMSBuildService msBuildService,
            IPipelineRunner pipelineRunner,
            IContainer mainContainer,
            INugetConfigurationService nugetConfigurationService,
            ILoggingService<PipelinesCE> loggingService)
        {
            _dependencyContextService = dependencyContextService;
            _mainContainer = mainContainer;
            _msBuildService = msBuildService;
            _pathService = pathService;
            _assemblyService = assemblyService;
            _pipelineRunner = pipelineRunner;
            _directoryService = directoryService;
            _loggingService = loggingService;
            _activatorService = activatorService;
            _nugetConfigurationService = nugetConfigurationService;
        }

        /// <summary>
        /// Runs pipeline specified by <see cref="PipelineOptions.Pipeline"/>
        /// </summary>
        /// <param name="pipelineOptions">
        /// Options provided by caller. Typically arguments to the command line application.
        /// </param>
        public virtual void Run(PipelineOptions pipelineOptions)
        {
            _loggingService.LogInformation(Strings.Log_InitializingPipelinesCE);

            string projectFile = _pathService.GetAbsolutePath(pipelineOptions.Project);

            // Build project
            _loggingService.LogInformation(Strings.Log_BuildingPipelinesCEProject, projectFile);
            _msBuildService.Build(projectFile, "/t:restore,build /p:configuration=release");
            _loggingService.LogInformation(Strings.Log_PipelinesCEProjectSuccessfullyBuilt, projectFile);

            // Load assemblies
            _loggingService.LogDebug(Strings.Log_LoadingAssemblies);
            IEnumerable<Assembly> assemblies = LoadAssemblies(projectFile);
            _loggingService.LogDebug(Strings.Log_AssembliesSuccessfullyLoaded);

            // Create plugin containers
            _loggingService.LogDebug(Strings.Log_BuildingPluginContainers);
            _pluginContainers = CreatePluginContainers(assemblies);
            _loggingService.LogDebug(Strings.Log_PluginContainersSuccessfullyBuilt);

            // Create pipeline factory
            _loggingService.LogInformation(Strings.Log_BuildingPipeline, pipelineOptions.Pipeline);
            IPipelineFactory factory = GetPipelineFactory(assemblies, pipelineOptions);
            _loggingService.LogInformation(Strings.Log_PipelineSuccessfullyBuilt, pipelineOptions.Pipeline);

            _loggingService.LogInformation(Strings.Log_PipelinesCESuccessfullyInitialized);

            // Create and run pipeline
            Pipeline pipeline = factory.CreatePipeline();
            pipeline.Options = pipelineOptions.Combine(pipeline.Options);
            _pipelineRunner.Run(pipeline, _pluginContainers);
        }

        // TODO Access modifier should be internal or private but no good way to test if so
        /// <summary>
        /// Loads assemblies from project represented by <paramref name="projectFile"/> that reference the PluginAndConfigTools 
        /// assembly
        /// </summary>
        /// <param name="projectFile"></param>
        /// <returns>
        /// <see cref="IEnumerable{Assembly}"/>
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if more than 1 *.deps.json file exists in publish directory
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if no *.deps.json file exists in publish directory
        /// </exception>
        public virtual IEnumerable<Assembly> LoadAssemblies(string projectFile)
        {
            string projectFileName = _pathService.GetFileNameWithoutExtension(projectFile);
            string projectDirectory = _directoryService.GetParent(projectFile).FullName;
            string publishDirectory = _pathService.Combine(projectDirectory, "bin/Release/netcoreapp1.1");

            // TODO what if framework version changes? can a wildcard be used? what if project builds for multiple frameworks?
            string[] possibleDepsFiles = _directoryService.GetFiles(publishDirectory, "*.deps.json", SearchOption.TopDirectoryOnly);
            if (possibleDepsFiles.Length > 1)
            {
                throw new InvalidOperationException(String.Format(Strings.Exception_MultipleDepsFiles, publishDirectory));
            }
            else if (possibleDepsFiles.Length == 0)
            {
                throw new InvalidOperationException(String.Format(Strings.Exception_NoDepsFiles, publishDirectory));
            }

            DependencyContext context = _dependencyContextService.CreateDependencyContext(possibleDepsFiles[0]);
            ISettings nugetSettings = _nugetConfigurationService.LoadDefaultSettings(projectDirectory);
            string globalPackagesFolder = _nugetConfigurationService.GetGlobalPackagesFolder(nugetSettings);
            _assemblyService.AddAssemblyDirectory(publishDirectory);
            _assemblyService.AddPackageCacheDirectory(globalPackagesFolder);

            // TODO what exactly is stored in a "repository", are the packages duplicated in the global cache? is null returned if no repository is used? 
            //string repositoryPath = _nugetConfigurationService.GetRepositoryPath(nugetSettings); 
            //if(repositoryPath != null)
            //_assemblyService.AddAssemblyDirectory(globalPackagesFolder);

            return _assemblyService.GetReferencingAssemblies(context, typeof(IPlugin).GetTypeInfo().Assembly);
        }

        // TODO Access modifier should be internal or private but no good way to test if so
        /// <summary>
        /// Creates an IOC container for each <see cref="IPlugin"/> implementation in <paramref name="assemblies"/>. Populates containers belonging to plugin types 
        /// with corresponding <see cref="IPluginStartup"/> implementations.
        /// </summary>
        /// <param name="assemblies"></param>
        public virtual IDictionary<string, IContainer> CreatePluginContainers(IEnumerable<Assembly> assemblies)
        {
            IDictionary<string, IContainer> result = new Dictionary<string, IContainer>();
            List<Type> pluginTypes = _assemblyService.GetAssignableTypes(assemblies, typeof(IPlugin)).ToList();
            IDictionary<string, Type> pluginStartupTypes = _assemblyService.GetAssignableTypes(assemblies, typeof(IPluginStartup)).ToDictionary(t => t.Name);
            foreach (Type pluginType in pluginTypes)
            {
                // Configure services
                _loggingService.LogDebug(Strings.Log_ConfiguringPluginContainer, pluginType.Name);
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
        public virtual IPipelineFactory GetPipelineFactory(IEnumerable<Assembly> assemblies, PipelineOptions pipelineOptions)
        {
            _loggingService.LogDebug(Strings.Log_RetrievingPipelineFactory, pipelineOptions.Pipeline);

            IEnumerable<Type> pipelineFactoryTypes = _assemblyService.
                GetAssignableTypes(assemblies, typeof(IPipelineFactory));

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
                    pipelineOptions.Pipeline = PipelineFactoryPipelineName(pipelineFactoryType); // Set PipelineOptions.Pipeline 
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
