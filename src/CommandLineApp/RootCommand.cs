using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.CommandLineUtils;
using System;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    /// <summary>
    /// Contains basic options such as --version and options shared by all commands, such as --verbose. All
    /// other commands are children of this command.
    /// </summary>
    public class RootCommand : CommandLineApplication
    {
        public CommandOption _verbose { get; }

        public RootCommand(PipelinesCE pipelinesCE, ICommandLineUtilsService cluService)
        {
            Description = "PipelinesCE, a continuous everything tool.";
            _verbose = Option("-o | --options",
                "Console output verbosity. If specified, outputs debug level logs.",
                CommandOptionType.NoValue,
                true);
            // TODO need to inject version into assembly on build
            VersionOption("-v | --version", "");

            Commands.Add(new RunCommand(pipelinesCE, cluService) { Parent = this });

            OnExecute((Func<int>)Run);
        }

        private int Run()
        {
            // TODO if version option provided, get and print version from pipelines CE
            return 0;
        }
    }
}
