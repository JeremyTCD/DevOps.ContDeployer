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
        private IPipelineRunner _pipelineRunner { get; }
        private ILogger<PipelinesCE> _logger { get; }

        public PipelinesCE(IProcessService processService,
            IAssemblyService assemblyService,
            IPipelineRunner pipelineRunner, 
            ILogger<PipelinesCE> logger)
        {
            _processService = processService;
            _assemblyService = assemblyService;
            _pipelineRunner = pipelineRunner;
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
            string projectFile = GetProjectFile(pipelineOptions.Project);
            string projectDirectory = Directory.GetParent(projectFile).FullName;

            BuildProject(projectFile);
            IPipelineFactory factory = GetPipelineFactory(projectDirectory, pipelineOptions.Pipeline);
            IEnumerable<IStep> steps = factory.CreatePipeline();

            _pipelineRunner.Run(steps, pipelineOptions);
        }

        private IPipelineFactory GetPipelineFactory(string projectDirectory, string pipeline)
        {
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

            IPipelineFactory factory = (IPipelineFactory)Activator.CreateInstance(pipelineFactoryType);
            return factory;
        }

        private void BuildProject(string projectFile)
        {
            string arguments = $"/t:restore,build /p:Configuration=Release {projectFile}";
            int result = _processService.Run("msbuild.exe", arguments);

            if (result != 0)
            {
                throw new Exception($"\"msbuild.exe\" process with arguments \"{arguments}\" failed");
            }
        }

        /// <summary>
        /// Gets file with name PipelinesCE project file."/>
        /// </summary>
        /// <param name="file">
        /// Name or path of project file. If a name is provided, PipelinesCE locates the file via a recursive search in the
        /// current directory.
        /// </param>
        /// <returns>
        /// File name
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if no file does not exist
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if more than 1 file exists in current directory or its children
        /// </exception>
        private string GetProjectFile(string file)
        {
            if (file.Contains('\\') || file.Contains('/'))
            {
                if (!Path.IsPathRooted(file))
                {
                    file = Path.Combine(Directory.GetCurrentDirectory(), file);
                }

                if (!File.Exists(file))
                {
                    throw new InvalidOperationException($"No file at path \"{file}\"");
                }
            }
            else
            {
                string directory = Directory.GetCurrentDirectory();
                string[] projectFiles = Directory.GetFiles(directory, file, SearchOption.AllDirectories);

                if (projectFiles.Length == 0)
                {
                    throw new InvalidOperationException($"No file with name \"{file}\" in directory \"{directory}\" or its children");
                }

                if (projectFiles.Length > 1)
                {
                    string message = $"Multiple files with name \"{file}\" found in directory \"{directory}\":\n";
                    foreach (string projectFile in projectFiles)
                    {
                        message += $"{projectFile}\n";
                    }
                    throw new InvalidOperationException(message);
                }
            }

            return file;
        }
}
}
