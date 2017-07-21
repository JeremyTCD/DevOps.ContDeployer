using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.Newtonsoft.Json.Utils;
using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using JeremyTCD.DotNetCore.ProjectRunner;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        private CommandLineAppOptions _claOptions { get; }
        private ICommandLineUtilsService _cluService { get; }
        private ILoggingService<RunCommand> _loggingService { get; }
        private IPathService _pathService { get; }
        private Runner _runner { get; }

        public RunCommand(ICommandLineUtilsService cluService, IOptions<CommandLineAppOptions> claOptionsAccessor, Runner runner,
            ILoggingService<RunCommand> loggingService, IPathService pathService)
        {
            _pathService = pathService;
            _claOptions = claOptionsAccessor.Value;
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
        }

        private int Run()
        {
            if (_loggingService.IsEnabled(LogLevel.Debug))
            {
                _loggingService.LogDebug(Strings.Log_RunningCommand,
                    Strings.CommandFullName_Run,
                    string.Join(Environment.NewLine, GetOptions().ToArray().Select(o => $"{o.LongName}={o.Value()}")));
            }

            PipelineOptions pipelineOptions = CreatePipelineOptions();
            string json = JsonConvert.SerializeObject(pipelineOptions, new PrivateFieldsJsonConverter());

            _runner.Run(_pathService.GetAbsolutePath(pipelineOptions.Project), 
                PipelineOptions.EntryAssemblyName, 
                PipelineOptions.EntryClassName,
                args: new string[] { json });

            return 0;
        }

        private PipelineOptions CreatePipelineOptions()
        {
            PipelineOptions pipelineOptions = new PipelineOptions
            {
                Project = _project.Value(),
                Pipeline = _pipeline.Value(),
            };

            if (_dryRun.HasValue())
            {
                pipelineOptions.DryRun = true;
            }
            else if (_dryRunOff.HasValue())
            {
                pipelineOptions.DryRun = false;
            }

            if (_verbose.HasValue())
            {
                pipelineOptions.Verbose = true;
            }
            else if (_verboseOff.HasValue())
            {
                pipelineOptions.Verbose = false;
            }

            return pipelineOptions;
        }
    }
}
