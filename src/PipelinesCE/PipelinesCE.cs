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
        private IProcessService _processService { get; }
        private IAssemblyService _assemblyService { get; }
        //private IPipelineRunner _pipelineRunner { get; }
        private ILogger<PipelinesCE> _logger { get; }
        private IPathService _pathService { get; }
        private IDirectoryService _directoryService { get; }
        private IMSBuildService _msBuildService { get; }

        public PipelinesCE(IProcessService processService,
            IAssemblyService assemblyService,
            IPathService pathService,
            IDirectoryService directoryService,
            IMSBuildService msBuildService,
            //IPipelineRunner pipelineRunner, 
            ILogger<PipelinesCE> logger)
        {
            _msBuildService = msBuildService;
            _pathService = pathService;
            _processService = processService;
            _assemblyService = assemblyService;
            //_pipelineRunner = pipelineRunner;
            _directoryService = directoryService;
            _logger = logger;
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
            string projectDirectory = _directoryService.GetParent(projectFile).FullName;

            _msBuildService.Build(projectFile, $"/t:restore,build /p:Configuration=Release {projectFile}");

            IPipelineFactory factory = GetPipelineFactory(projectDirectory, pipelineOptions.Pipeline);
            IEnumerable<IStep> steps = factory.CreatePipeline();

            // TODO create a container for each plugin 

            //_pipelineRunner.Run(steps, pipelineOptions);
        }

        private IPipelineFactory GetPipelineFactory(string projectDirectory, string pipeline)
        {
            // TODO handle case where pipeline is null
            // TODO what if framework version changes?
            IEnumerable<Assembly> assemblies = _assemblyService.LoadAssembliesInDir(Path.Combine(projectDirectory, "bin/Releases/netcoreapp1.1"), true);
            IDictionary<string, Type> pipelineFactoryTypes = _assemblyService.
                GetAssignableTypes(assemblies, typeof(IPipelineFactory)).
                ToDictionary(t => t.Name.Replace("PipelineFactory", ""));

            // TODO if Pipeline is null
            pipelineFactoryTypes.TryGetValue(pipeline, out Type pipelineFactoryType);

            if (pipelineFactoryType == null)
            {
                throw new InvalidOperationException($"No pipeline with name \"{pipeline}\"");
            }

            // TODO ActivatorService
            IPipelineFactory factory = (IPipelineFactory)Activator.CreateInstance(pipelineFactoryType);
            return factory;
        }
    }
}
