using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace JeremyTCD.PipelinesCE
{
    public class PipelinesCE
    {
        private IAssemblyService _assemblyService { get; }
        private IPipelineRunner _pipelineRunner { get; }
        private ILogger<PipelinesCE> _logger { get; }
        private IPathService _pathService { get; }
        private IDirectoryService _directoryService { get; }
        private IMSBuildService _msBuildService { get; }
        private IActivatorService _activatorService { get; }

        public PipelinesCE(IActivatorService activatorService,
            IAssemblyService assemblyService,
            IPathService pathService,
            IDirectoryService directoryService,
            IMSBuildService msBuildService,
            IPipelineRunner pipelineRunner,
            ILogger<PipelinesCE> logger)
        {
            _msBuildService = msBuildService;
            _pathService = pathService;
            _assemblyService = assemblyService;
            _pipelineRunner = pipelineRunner;
            _directoryService = directoryService;
            _logger = logger;
            _activatorService = activatorService;
        }

        /// <summary>
        /// Runs pipeline specified by <see cref="PipelineOptions.Pipeline"/>
        /// </summary>
        /// <param name="pipelineOptions">
        /// Options provided by caller. Typically arguments to the command line application.
        /// </param>
        public virtual void Run(PipelineOptions pipelineOptions)
        {
            string projectFile = _pathService.GetAbsolutePath(pipelineOptions.Project);

            _msBuildService.Build(projectFile, Strings.PipelinesCEProjectMSBuildSwitches);

            IPipelineFactory factory = GetPipelineFactory(projectFile, pipelineOptions.Pipeline);
            Pipeline pipeline = factory.CreatePipeline();
            pipeline.Options = pipelineOptions.Combine(pipeline.Options);


            // TODO create a container for each plugin 

            _pipelineRunner.Run(pipeline);
        }

        /// <summary>
        /// Gets <see cref="IPipelineFactory"/> from project defined by <paramref name="projectFile"/> that creates a pipeline with name 
        /// <paramref name="pipeline"/>. If <paramref name="pipeline"/> is null and there is only one <see cref="IPipelineFactory"/> 
        /// implementation, returns an instance of the sole implementation.
        /// </summary>
        /// <param name="projectFile"></param>
        /// <param name="pipeline"></param>
        /// <returns>
        /// <see cref="IPipelineFactory"/>
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if project does not contain any <see cref="IPipelineFactory"/> implementations
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <paramref name="pipeline"/> is null and there are multiple pipeline factories
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if no <see cref="IPipelineFactory"/> produces a pipeline with name <paramref name="pipeline"/>
        /// </exception>
        private IPipelineFactory GetPipelineFactory(string projectFile, string pipeline)
        {
            string projectDirectory = _directoryService.GetParent(projectFile).FullName;

            // TODO what if framework version changes? can a wildcard be used? what if project builds for multiple frameworks?
            IEnumerable<Assembly> assemblies = _assemblyService.LoadAssembliesInDir(Path.Combine(projectDirectory, "bin/Releases/netcoreapp1.1"), true);
            IDictionary<string, Type> pipelineFactoryTypes = _assemblyService.
                GetAssignableTypes(assemblies, typeof(IPipelineFactory)).
                ToDictionary(t => t.Name.Replace("PipelineFactory", "").ToLowerInvariant());

            if (pipelineFactoryTypes.Count == 0)
            {
                throw new InvalidOperationException(string.Format(Strings.NoPipelineFactories, projectFile));
            }

            Type pipelineFactoryType;
            if (pipeline == null)
            {
                if (pipelineFactoryTypes.Count == 1)
                {
                    pipelineFactoryType = pipelineFactoryTypes.First().Value;
                }
                else
                {
                    throw new InvalidOperationException(string.Format(Strings.MultiplePipelineFactories, 
                        string.Join("\n", pipelineFactoryTypes.Values)));
                }
            }
            else
            {
                pipelineFactoryTypes.TryGetValue(pipeline.ToLowerInvariant(), out pipelineFactoryType);
                if (pipelineFactoryType == null)
                {
                    throw new InvalidOperationException(string.Format(Strings.NoPipelineFactory, pipeline));
                }
            }

            IPipelineFactory factory = (IPipelineFactory)_activatorService.CreateInstance(pipelineFactoryType);
            return factory;
        }
    }
}
