using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    // TODO each command can actually be tested individually
    public class RunCommand : CommandLineApplication
    {
        private CommandOption _project { get; set; }
        private CommandOption _pipeline { get; set; }
        private CommandOption _dryRun { get; set; }
        public CommandOption _verbose { get; set; }

        private CommandLineAppOptions _claOptions { get; }
        private PipelinesCE _pipelinesCE { get; }
        private ICommandLineUtilsService _cluService { get; }
        private ILoggerFactory _loggerFactory { get; }

        public RunCommand(PipelinesCE pipelinesCE, ICommandLineUtilsService cluService, ILoggerFactory loggerFactory,
            IOptions<CommandLineAppOptions> claOptionsAccessor)
        {
            _claOptions = claOptionsAccessor.Value;
            _loggerFactory = loggerFactory;
            _pipelinesCE = pipelinesCE;
            _cluService = cluService;

            Description = Strings.RunCommandDescription;
            Name = nameof(RunCommand).Replace("Command", "").ToLowerInvariant();
            FullName = $"{nameof(PipelinesCE)} {nameof(RunCommand).Replace("Command", "")}";
            SetupOptions();
            OnExecute((Func<int>)Run);
        }

        private void SetupOptions()
        {
            HelpOption(_cluService.CreateOptionTemplate(Strings.HelpOptionShortName, Strings.HelpOptionLongName));

            _project = Option(_cluService.CreateOptionTemplate(Strings.ProjectOptionShortName, Strings.ProjectOptionLongName),
                Strings.ProjectOptionDescription,
                CommandOptionType.SingleValue);
            _pipeline = Option(_cluService.CreateOptionTemplate(Strings.PipelineOptionShortName, Strings.PipelineOptionLongName),
                Strings.PipelineOptionDescription,
                CommandOptionType.SingleValue);
            _dryRun = Option(_cluService.CreateOptionTemplate(Strings.DryRunOptionShortName, Strings.DryRunOptionLongName),
                Strings.DryRunDescription,
                CommandOptionType.NoValue);
            _verbose = Option(_cluService.CreateOptionTemplate(Strings.VerboseOptionShortName, Strings.VerboseOptionLongName),
                Strings.VerboseOptionDescription,
                CommandOptionType.NoValue);
        }

        private int Run()
        {
            // TODO verify that configuring logger factory here affects loggers in _pipelinesCE's dependency tree
            LogLevel logLevel = _verbose.HasValue() ? _claOptions.VerboseMinLogLevel : _claOptions.DefaultMinLogLevel;

            _loggerFactory.
                AddFile(_claOptions.LogFileFormat, logLevel).
                AddConsole(logLevel).
                AddDebug(logLevel);

            PipelineOptions pipelineOptions = new PipelineOptions
            {
                DryRun = _dryRun.HasValue(),
                Project = _project.Value(),
                Pipeline = _pipeline.Value()
            };

            _pipelinesCE.Run(pipelineOptions);

            return 0;
        }
    }
}
