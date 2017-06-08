using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StructureMap;
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
        private ICommandLineUtilsService _cluService { get; }
        private IContainer _container { get; }

        public RunCommand(ICommandLineUtilsService cluService, IOptions<CommandLineAppOptions> claOptionsAccessor, IContainer container)
        {
            _claOptions = claOptionsAccessor.Value;
            _cluService = cluService;
            _container = container;

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
            // Configure logging
            // PipelinesCE and its plugins simply log using microsoft.extensions.logging. Calling libraries or
            // applications determine what providers to use and the verbosity level.
            ILoggerFactory loggerFactory = _container.GetInstance<ILoggerFactory>();
            LogLevel logLevel = _verbose.HasValue() ? _claOptions.VerboseMinLogLevel : _claOptions.DefaultMinLogLevel;
            loggerFactory.
                AddFile(_claOptions.LogFileFormat, logLevel).
                AddConsole(logLevel).
                AddDebug(logLevel);

            PipelinesCE pipelinesCE = _container.GetInstance<PipelinesCE>();
            pipelinesCE.Run(new PipelineOptions
            {
                DryRun = _dryRun.HasValue(),
                Project = _project.Value(),
                Pipeline = _pipeline.Value()
            });

            return 0;
        }
    }
}
