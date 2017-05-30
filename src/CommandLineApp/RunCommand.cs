using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.CommandLineUtils;
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

        private PipelinesCE _pipelinesCE { get; }
        private ICommandLineUtilsService _cluService { get; }

        public RunCommand(PipelinesCE pipelinesCE, ICommandLineUtilsService cluService)
        {
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
            PipelineOptions pipelineOptions = new PipelineOptions
            {
                Verbose = _verbose.HasValue(),
                DryRun = _dryRun.HasValue(),
                Project = _project.Value(),
                Pipeline = _pipeline.Value()
            };

            _pipelinesCE.Run(pipelineOptions);

            return 0;
        }
    }
}
