using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace JeremyTCD.PipelinesCE.Plugin.MSBuild
{
    public class MSBuildPlugin : IPlugin
    {
        private IMSBuildClient _msBuildClient { get; }

        public MSBuildPlugin(IMSBuildClient msBuildClient)
        {
            _msBuildClient = msBuildClient;
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

            if (!pipelineContext.SharedOptions.DryRun)
            {
                _msBuildClient.Build(options.ProjOrSlnFile, options.Switches);
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
