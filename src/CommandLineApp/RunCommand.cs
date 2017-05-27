using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.CommandLineUtils;
using System;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    // TODO each command can actually be tested individually
    public class RunCommand : CommandLineApplication
    {
        private CommandOption _project { get; }
        private CommandOption _pipeline { get; }
        private CommandOption _dryRun { get; }
        private PipelinesCE _pipelinesCE { get; }

        public RunCommand(PipelinesCE pipelinesCE)
        {
            Description = "Runs a pipeline. Usage: \"pipelinesce run [-pj <project>] [-pl <pipeline>]\"";

            _project = Option("-pj | --project",
                "Project file (.csproj) of project used to define pipeline(s). If unspecified, searches recursively for PipelinesCE.csproj in the current directory.",
                CommandOptionType.SingleValue);
            _pipeline = Option("-pl | --pipeline",
                "Name of pipeline to run. By convention, a pipeline's name is its factories name less \"PipelineFactory\". For example, the name of the pipeline produced by" +
                "\"MainPipelineFactory\" is \"Main\". If unspecified and only one pipeline has been defined, runs said pipeline.",
                CommandOptionType.SingleValue);
            _dryRun = Option("-d | --dryrun",
                "If included, runs pipeline in dry run mode.",
                CommandOptionType.NoValue);

            _pipelinesCE = pipelinesCE;
            HelpOption("-? | -h | --help");

            OnExecute((Func<int>)Run);
        }

        private int Run()
        {
            DefaultCommand defaultCommand = (DefaultCommand)Parent;

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
