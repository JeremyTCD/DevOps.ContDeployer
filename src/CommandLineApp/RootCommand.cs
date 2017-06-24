using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    /// <summary>
    /// Contains basic options such as --version and options shared by all commands, such as --verbose. All
    /// other commands are children of this command.
    /// </summary>
    public class RootCommand : CommandLineApplication
    {
        private ICommandLineUtilsService _cluService { get; }
        private ILoggingService<RunCommand> _loggingService { get; }
        private CommandOption _verbose { get; set; }

        public RootCommand(ICommandLineUtilsService cluService, RunCommand runCommand, ILoggingService<RunCommand> loggingService)

        {
            _cluService = cluService;
            _loggingService = loggingService;

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
            _verbose = Option(_cluService.CreateOptionTemplate(Strings.OptionShortName_Verbose, Strings.OptionLongName_Verbose),
                Strings.OptionDescription_Verbose,
                CommandOptionType.NoValue, true);
        }

        private int Run()
        {
            if (_loggingService.IsEnabled(LogLevel.Debug))
            {
                _loggingService.LogDebug(Strings.Log_RunningCommand, Strings.CommandFullName_Root, string.Join(Environment.NewLine, Options.ToArray().Select(o => $"{o.LongName}={o.Value()}")));
            }

            ShowHelp();

            return 0;
        }
    }
}
