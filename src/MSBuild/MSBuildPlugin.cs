using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using System;

namespace JeremyTCD.PipelinesCE.Plugin.MSBuild
{
    /// <summary>
    /// This plugin runs MSBuild.exe with arguments specified in an <see cref="MSBuildPluginOptions"/> instance.
    /// 
    /// Note that this plugin will eventually be superceded by Roslyn, Nuget and MSBuild-target replacing plugins.
    /// Also note that this plugin does not attempt to install MSBuild.exe.
    /// </summary>
    public class MSBuildPlugin : IPlugin
    {
        private IMSBuildService _msBuildService { get; }
        private ILoggingService<MSBuildPlugin> _loggingService { get; }
        private IDirectoryService _directoryService { get; }

        public MSBuildPlugin(IMSBuildService msBuildService, 
            ILoggingService<MSBuildPlugin> loggingService,
            IDirectoryService directoryService)
        {
            _msBuildService = msBuildService;
            _loggingService = loggingService;
            _directoryService = directoryService;
        }

        /// <summary>
        /// Runs MSBuild.exe with arguments specified in an <see cref="MSBuildPluginOptions"/> instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="IStepContext.PluginOptions"/> is null
        /// </exception>
        public void Run(IPipelineContext pipelineContext, IStepContext stepContext)
        {
            MSBuildPluginOptions options = stepContext.PluginOptions as MSBuildPluginOptions;

            if (options == null)
            {
                throw new InvalidOperationException($"{nameof(MSBuildPluginOptions)} required");
            }

            if (!pipelineContext.PipelineOptions.DryRun)
            {
                _msBuildService.Build(options.ProjOrSlnFile, options.Switches);
            }

            if (string.IsNullOrEmpty(options.ProjOrSlnFile))
            {
                _loggingService.
                    LogInformation(Strings.Log_RanMSBuildInDir, _directoryService.GetCurrentDirectory(), options.Switches);
            }
            else
            {
                _loggingService.
                    LogInformation(Strings.Log_RanMSBuildOnFile, options.ProjOrSlnFile, options.Switches);
            }
        }
    }
}
