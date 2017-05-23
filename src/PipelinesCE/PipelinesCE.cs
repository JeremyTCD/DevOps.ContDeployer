using JeremyTCD.PipelinesCE.PluginTools;
using System;
using System.IO;

namespace JeremyTCD.PipelinesCE
{
    public class PipelinesCE
    {
        private IProcessManager _processManager { get; }

        // TODO process manager should be moved to dotnet utils, plugin tools can refernce it
        public PipelinesCE(IProcessManager processManager)
        {
            _processManager = processManager;
        }

        public void Start()
        {
            string projectFile = GetPipelinesCEProjectFile();
            BuildPipelinesCEProject(projectFile);
            // Depending on console app args, create relevant pipelines using their PipelineFactory instances
            // Execute relevant pipelines
        }

        private void BuildPipelinesCEProject(string projectFile)
        {
            _processManager.Execute("msbuild.exe", $"/p:Configuration=Release {projectFile}");
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

            if(projectFiles.Length == 0)
            {
                throw new InvalidOperationException($"No file with name \"{fileName}\" in directory \"{directory}\" or its children");
            }

            if(projectFiles.Length > 1)
            {
                string message = $"Multiple files with name \"{fileName}\" found in directory \"{directory}\":\n";
                foreach(string file in projectFiles)
                {
                    message += $"{file}\n";
                }
                throw new InvalidOperationException(message);
            }

            return projectFiles[0];
        }
    }
}  
