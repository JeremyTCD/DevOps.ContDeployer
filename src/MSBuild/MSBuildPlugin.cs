using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace JeremyTCD.ContDeployer.Plugin.MSBuild
{
    public class MSBuildPlugin : IPlugin
    {
        private MSBuildPluginOptions _options { get; }
        private IMSBuildClient _msBuildClient { get; }

        public MSBuildPlugin(IMSBuildClient msBuildClient, IPipelineContext pipelineContext, IStepContext stepContext) :
            base(pipelineContext, stepContext)
        {
            _options = stepContext.PluginOptions as MSBuildPluginOptions;

            if(_options == null)
            {
                throw new InvalidOperationException($"{nameof(MSBuildPluginOptions)} required");
            }

            _msBuildClient = msBuildClient;
        }

        /// <summary>
        /// Executes MSBuild.exe with specified arguments
        /// </summary>
        public void Run(IPipelineContext pipelineContext, IStepContext stepContext)
        {
            if (!PipelineContext.SharedOptions.DryRun)
            {
                _msBuildClient.Build(_options.ProjOrSlnFile, _options.Switches);
            }

            string logMessage = String.Concat("MSBuild.exe executed",
                _options.ProjOrSlnFile == null ? $"in { Directory.GetCurrentDirectory()}" :
                    $"on file \"{_options.ProjOrSlnFile}\"", 
                _options.Switches == null ? "with no switches" : $"with switches \"{_options.Switches}\"",
                ".");

            StepContext.
                Logger.
                LogInformation(logMessage);
        }
    }
}
