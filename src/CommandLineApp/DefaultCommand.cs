using Microsoft.Extensions.CommandLineUtils;
using System;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    public class DefaultCommand : CommandLineApplication
    {
        public CommandOption _verbose { get; }

        public DefaultCommand(PipelinesCE pipelinesCE, RunCommand runCommand)
        {
            Description = "PipelinesCE, a continuous everything tool.";
            _verbose = Option("-o | --options",
                "Console output verbosity. If specified, outputs debug level logs.",
                CommandOptionType.NoValue,
                true);
            // TODO need to inject version into assembly on build
            VersionOption("-v | --version", "");

            Commands.Add(runCommand);

            OnExecute((Func<int>)Run);
        }

        private int Run()
        {
            // TODO if version option provided, get and print version from pipelines CE
            return 0;
        }
    }
}
