using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace JeremyTCD.PipelinesCE.Plugin.MSBuild
{
    public class MSBuildPlugin : IPlugin
    {
        private IMSBuildService _msBuildService { get; }

        public MSBuildPlugin(IMSBuildService msBuildService)
        {
            _msBuildService = msBuildService;
        }

        /// <summary>
        /// Executes MSBuild.exe with specified arguments
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="IStepContext.PluginOptions"/> is null
        /// </exception>
        public void Run(IPipelineContext pipelineContext, IStepContext stepContext)
        {
            MSBuildPluginOptions options = stepContext.PluginOptions as MSBuildPluginOptions;

            if(options == null)
            {
                throw new InvalidOperationException($"{nameof(MSBuildPluginOptions)} required");
            }

            if (!pipelineContext.PipelineOptions.DryRun)
            {
                _msBuildService.Build(options.ProjOrSlnFile, options.Switches);
            }

            string logMessage = String.Concat("MSBuild.exe executed",
                options.ProjOrSlnFile == null ? $"in { Directory.GetCurrentDirectory()}" :
                    $"on file \"{options.ProjOrSlnFile}\"", 
                options.Switches == null ? "with no switches" : $"with switches \"{options.Switches}\"",
                ".");

            stepContext.
                Logger.
                LogInformation(logMessage);
        }
    }
}
