
using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using StructureMap;
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
        private IContainer _mainContainer { get; }

        public PipelinesCE(IProcessService processService, IAssemblyService assemblyService, IContainer mainContainer)
        {
            _processService = processService;
            _assemblyService = assemblyService;
            _mainContainer = mainContainer;
        }

        /// <summary>
        /// Runs pipeline specified by <see cref="PipelineOptions.Pipeline"/>
        /// </summary>
        /// <param name="pipelineOptions">
        /// Options provided by caller. Typically arguments to the command line application.
        /// </param>
        public void Run(PipelineOptions pipelineOptions)
        {
            string projectFile = GetPipelinesCEProjectFile();
            string projectDirectory = Directory.GetParent(projectFile).FullName;

            BuildPipelinesCEProject(projectFile);
            IDictionary<string, Type> pipelineFactoryTypes = GetPipelineFactoryTypes(projectDirectory);

            List<IPipeline> pipelines = new List<IPipeline>();
            pipelineFactoryTypes.TryGetValue(pipelineOptions.Pipeline, out Type type);

            if (type == null)
            {
                throw new InvalidOperationException($"No pipeline with name \"{pipelineOptions.Pipeline}\"");
            }
            else
            {
                IPipelineFactory factory = (IPipelineFactory)Activator.CreateInstance(type);
                IEnumerable<IStep> steps = factory.CreatePipeline();
                IPipeline pipeline = _mainContainer.GetInstance<IPipeline>();

                pipeline.Run(steps, pipelineOptions);
            }
        }

        private IDictionary<string, Type> GetPipelineFactoryTypes(string projectDirectory)
        {
            // TODO what if framework version changes?
            IEnumerable<Assembly> assemblies = _assemblyService.LoadAssembliesInDir(Path.Combine(projectDirectory, "bin/Releases/netcoreapp1.1"), true);
            IEnumerable<Type> pipelineFactoryTypes = _assemblyService.GetAssignableTypes(assemblies, typeof(IPipelineFactory));

            return pipelineFactoryTypes.ToDictionary(t => t.Name.Replace("PipelineFactory", ""));
        }

        private void BuildPipelinesCEProject(string projectFile)
        {
            string arguments = $"/t:restore,build /p:Configuration=Release {projectFile}";
            int result = _processService.Run("msbuild.exe", arguments);

            if (result != 0)
            {
                throw new Exception($"\"msbuild.exe\" process with arguments \"{arguments}\" failed");
            }
        }

        /// <summary>
        /// Gets file with name PipelinesCE.csproj. Looks in current directory and its children."/>
        /// </summary>
        /// <returns>
        /// File name
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if no file with name PipelinesCE.csproj exists in current directory or its children.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if more than 1 files with name PipelinesCE.csproj exists in current directory or its children.
        /// </exception>
        private string GetPipelinesCEProjectFile()
        {
            // Should eventually be specifiable
            string fileName = "PipelinesCE.csproj";
            string directory = Directory.GetCurrentDirectory();
            string[] projectFiles = Directory.GetFiles(directory, fileName, SearchOption.AllDirectories);

            if (projectFiles.Length == 0)
            {
                throw new InvalidOperationException($"No file with name \"{fileName}\" in directory \"{directory}\" or its children");
            }

            if (projectFiles.Length > 1)
            {
                string message = $"Multiple files with name \"{fileName}\" found in directory \"{directory}\":\n";
                foreach (string file in projectFiles)
                {
                    message += $"{file}\n";
                }
                throw new InvalidOperationException(message);
            }

            return projectFiles[0];
        }
    }
}
