using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Reflection;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    /// <summary>
    /// Contains basic options such as --version and options shared by all commands, such as --verbose. All
    /// other commands are children of this command.
    /// </summary>
    public class RootCommand : CommandLineApplication
    {
        private PipelinesCE _pipelinesCE { get; }
        private ICommandLineUtilsService _cluService { get; }

        public RootCommand(PipelinesCE pipelinesCE, ICommandLineUtilsService cluService,
            RunCommand runCommand)
        {
            _cluService = cluService;
            _pipelinesCE = pipelinesCE;

            Description = Strings.RunCommandDescription;
            Name = nameof(PipelinesCE).ToLowerInvariant();
            FullName = nameof(PipelinesCE);
            SetupCommands(runCommand);
            SetupOptions();
            OnExecute((Func<int>)Run);
        }

        private void SetupCommands(RunCommand runCommand)
        {
            runCommand.Parent = this;
            Commands.Add(runCommand);
        }

        private void SetupOptions()
        {
            HelpOption(_cluService.CreateOptionTemplate(Strings.HelpOptionShortName, Strings.HelpOptionLongName));
            VersionOption(_cluService.CreateOptionTemplate(Strings.VersionOptionShortName, Strings.VersionOptionLongName),
                typeof(RootCommand).GetTypeInfo().Assembly.GetName().Version.ToString());
        }

        private int Run()
        {
            ShowHelp();

            return 0;
        }
    }
}
