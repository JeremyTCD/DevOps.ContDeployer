using JeremyTCD.DotNetCore.ProjectHost;
using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.Newtonsoft.Json.Utils;
using JeremyTCD.PipelinesCE.Core;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    public class RunCommand : CommandLineApplication
    {
        private CommandOption _project { get; set; }
        private CommandOption _pipeline { get; set; }
        private CommandOption _dryRun { get; set; }
        private CommandOption _dryRunOff { get; set; }
        private CommandOption _verbose { get; set; }
        private CommandOption _verboseOff { get; set; }
        private CommandOption _debug { get; set; }
        private CommandOption _debugOff { get; set; }

        private ICommandLineUtilsService _cluService { get; }
        private ILoggingService<RunCommand> _loggingService { get; }
        private IPathService _pathService { get; }
        private ProjectRunner _runner { get; }

        public RunCommand(ICommandLineUtilsService cluService, ProjectRunner runner, ILoggingService<RunCommand> loggingService, IPathService pathService)
        {
            _pathService = pathService;
            _cluService = cluService;
            _runner = runner;
            _loggingService = loggingService;

            Description = Strings.CommandDescription_Run;
            Name = Strings.CommandName_Run;
            FullName = Strings.CommandFullName_Run;
            SetupOptions();
            OnExecute((Func<int>)Run);
        }

        private void SetupOptions()
        {
            HelpOption(_cluService.CreateOptionTemplate(Strings.OptionShortName_Help, Strings.OptionLongName_Help));

            _project = Option(_cluService.CreateOptionTemplate(Strings.OptionShortName_Project, Strings.OptionLongName_Project),
                Strings.OptionDescription_Project,
                CommandOptionType.SingleValue);
            _pipeline = Option(_cluService.CreateOptionTemplate(Strings.OptionShortName_Pipeline, Strings.OptionLongName_Pipeline),
                Strings.OptionDescription_Pipeline,
                CommandOptionType.SingleValue);
            _dryRun = Option(_cluService.CreateOptionTemplate(Strings.OptionShortName_DryRun, Strings.OptionLongName_DryRun),
                Strings.OptionDescription_DryRun,
                CommandOptionType.NoValue);
            _dryRunOff = Option(_cluService.CreateOptionTemplate(Strings.OptionShortName_DryRunOff, Strings.OptionLongName_DryRunOff),
                Strings.OptionDescription_DryRunOff,
                CommandOptionType.NoValue);
            _verbose = Option(_cluService.CreateOptionTemplate(Strings.OptionShortName_Verbose, Strings.OptionLongName_Verbose),
                Strings.OptionDescription_Verbose,
                CommandOptionType.NoValue);
            _verboseOff = Option(_cluService.CreateOptionTemplate(Strings.OptionShortName_VerboseOff, Strings.OptionLongName_VerboseOff),
                Strings.OptionDescription_VerboseOff,
                CommandOptionType.NoValue);
            _debug = Option(_cluService.CreateOptionTemplate(Strings.OptionShortName_Debug, Strings.OptionLongName_Debug),
                Strings.OptionDescription_Debug,
                CommandOptionType.NoValue);
            _debugOff = Option(_cluService.CreateOptionTemplate(Strings.OptionShortName_DebugOff, Strings.OptionLongName_DebugOff),
                Strings.OptionDescription_DebugOff,
                CommandOptionType.NoValue);
        }

        private int Run()
        {
            ShowVersion();
            if (_loggingService.IsEnabled(LogLevel.Debug))
            {
                _loggingService.LogDebug(Strings.Log_RunningCommand,
                    Strings.CommandFullName_Run,
                    string.Join(Environment.NewLine, GetOptions().ToArray().Select(o => $"{o.LongName}={o.Value()}")));
            }

            // Process CommandOptions
            PipelinesCEOptions pipelinesCEOptions = CreatePipelinesCEOptions();
            SharedPluginOptions sharedPluginOptions = CreateSharedPluginOptions();
            // Serialize options
            PrivateFieldsJsonConverter pfjc = new PrivateFieldsJsonConverter();
            string pipelinesCEOptionsJson = JsonConvert.SerializeObject(pipelinesCEOptions, pfjc);
            string sharedPluginOptionsJson = JsonConvert.SerializeObject(sharedPluginOptions, pfjc);

            return _runner.Run(_pathService.GetAbsolutePath(pipelineOptions.Project),
                PipelineOptions.EntryAssemblyName,
                PipelineOptions.EntryClassName,
                args: new string[] { pipelinesCEOptionsJson, sharedPluginOptionsJson });
        }

        private SharedPluginOptions CreateSharedPluginOptions()
        {
            SharedPluginOptions sharedPluginOptions = new SharedPluginOptions();

            if (_dryRun.HasValue())
                sharedPluginOptions.DryRun = true;
            else if (_dryRunOff.HasValue())
                sharedPluginOptions.DryRun = false;

            return sharedPluginOptions;
        }

        private PipelinesCEOptions CreatePipelinesCEOptions()
        {
            PipelinesCEOptions pipelineOptions = new PipelinesCEOptions
            {
                Project = _project.Value(),
                Pipeline = _pipeline.Value(),
            };

            if (_verbose.HasValue())
                pipelineOptions.Verbose = true;
            else if (_verboseOff.HasValue())
                pipelineOptions.Verbose = false;

            if (_debug.HasValue())
                pipelineOptions.Debug = true;
            else if (_debugOff.HasValue())
                pipelineOptions.Debug = false;

            return pipelineOptions;
        }
    }
}
