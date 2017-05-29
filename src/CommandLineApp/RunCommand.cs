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

        private PipelinesCE _pipelinesCE { get; }
        private ICommandLineUtilsService _cluService { get; }

        public RunCommand(PipelinesCE pipelinesCE, ICommandLineUtilsService cluService)
        {
            _pipelinesCE = pipelinesCE;
            _cluService = cluService;

            Description = Strings.RunCommandDescription;
            Name = nameof(RunCommand).Replace("Command", "").ToLowerInvariant();
            SetupOptions();
            OnExecute((Func<int>)Run);
        }

        private void SetupOptions()
        {
            _project = Option(_cluService.CreateOptionTemplate(Strings.ProjectOptionShortName, Strings.ProjectOptionLongName),
                Strings.ProjectOptionDescription,
                CommandOptionType.SingleValue);
            _pipeline = Option(_cluService.CreateOptionTemplate(Strings.PipelineOptionShortName, Strings.PipelineOptionLongName),
                Strings.ProjectOptionDescription,
                CommandOptionType.SingleValue);
            _dryRun = Option(_cluService.CreateOptionTemplate(Strings.DryRunOptionShortName, Strings.DryRunOptionLongName),
                Strings.DryRunDescription,
                CommandOptionType.NoValue);
            HelpOption(_cluService.CreateOptionTemplate(Strings.HelpOptionShortName, Strings.HelpOptionLongName, Strings.HelpOptionSymbolName));
        }

        private int Run()
        {
            RootCommand defaultCommand = (RootCommand)Parent;

            PipelineOptions pipelineOptions = new PipelineOptions
            {
                Verbose = defaultCommand._verbose.HasValue(),
                DryRun = _dryRun.HasValue(),
                Project = _project.Value(),
                Pipeline = _pipeline.Value()
            };

            // TODO get options from command line arguments
            _pipelinesCE.Run(pipelineOptions);

            return 0;
        }
    }
}
