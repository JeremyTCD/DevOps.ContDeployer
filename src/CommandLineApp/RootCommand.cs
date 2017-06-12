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

        public RootCommand(ICommandLineUtilsService cluService, RunCommand runCommand, ILoggingService<RunCommand> loggingService)

        {
            _cluService = cluService;
            _loggingService = loggingService;

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
            if (_loggingService.IsEnabled(LogLevel.Debug))
            {
                _loggingService.LogDebug(Strings.Log_RunningRunCommand, string.Join("\n", Options.ToArray().Select(o => $"{o.LongName}={o.Value()}")));
            }

            ShowHelp();

            return 0;
        }
    }
}
