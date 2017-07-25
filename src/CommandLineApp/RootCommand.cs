using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Reflection;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    /// <summary>
    /// Contains basic options such as --version --help. All other commands are children of this command.
    /// </summary>
    public class RootCommand : CommandLineApplication
    {
        private ICommandLineUtilsService _cluService { get; }

        public RootCommand(ICommandLineUtilsService cluService, RunCommand runCommand)

        {
            _cluService = cluService;

            Description = Strings.CommandDescription_Run;
            Name = Strings.CommandName_Root;
            FullName = Strings.CommandFullName_Root;
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
            HelpOption(_cluService.CreateOptionTemplate(Strings.OptionShortName_Help, Strings.OptionLongName_Help));
            VersionOption(_cluService.CreateOptionTemplate(Strings.OptionShortName_Version, Strings.OptionLongName_Version),
                typeof(RootCommand).GetTypeInfo().Assembly.GetName().Version.ToString());
        }

        private int Run()
        {
            ShowHelp(); // User enters just "PipelinesCE"

            return 0;
        }
    }
}
