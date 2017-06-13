using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StructureMap;
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
        private IContainer _container { get; }
        private ILoggingService<RunCommand> _loggingService { get; }

        public RunCommand(ICommandLineUtilsService cluService, IOptions<CommandLineAppOptions> claOptionsAccessor, IContainer container,
            ILoggingService<RunCommand> loggingService)
        {
            _claOptions = claOptionsAccessor.Value;
            _cluService = cluService;
            _container = container;
            _loggingService = loggingService;

            Description = Strings.CommandDescription_Run;
            Name = nameof(RunCommand).Replace("Command", "").ToLowerInvariant();
            FullName = $"{nameof(PipelinesCE)} {nameof(RunCommand).Replace("Command", "")}";
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

        // Questions
        // If optiontype is novalue and a value is provided, is an error thrown? is value made available?
        // If optiontype is singlevalue and no value is provided, is an error thrown?
        private int Run()
        {
            if (_loggingService.IsEnabled(LogLevel.Debug))
            {
                _loggingService.LogDebug(Strings.Log_RunningRunCommand, string.Join(Environment.NewLine, Options.ToArray().Select(o => $"{o.LongName}={o.Value()}")));
            }

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

            // Configure logging
            // PipelinesCE and its plugins simply log using microsoft.extensions.logging. Calling libraries or
            // applications determine what providers to use and the verbosity level.
            ILoggerFactory loggerFactory = _container.GetInstance<ILoggerFactory>();
            LogLevel logLevel = pipelineOptions.Verbose ? _claOptions.VerboseMinLogLevel : _claOptions.DefaultMinLogLevel;
            loggerFactory.
                AddFile(_claOptions.LogFileFormat, logLevel).
                AddConsole(logLevel).
                AddDebug(logLevel);

            PipelinesCE pipelinesCE = _container.GetInstance<PipelinesCE>();
            pipelinesCE.Run(pipelineOptions);

            return 0;
        }
    }
}
