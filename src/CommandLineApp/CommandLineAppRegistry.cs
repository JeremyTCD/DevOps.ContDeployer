using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.ProjectRunner;
using StructureMap;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    public class CommandLineAppRegistry : Registry
    {
        public CommandLineAppRegistry()
        {
            IncludeRegistry<ProjectRunnerRegistry>();

            For<ICommandLineUtilsService>().Singleton().Use<CommandLineUtilsService>();

            For<RunCommand>().Singleton().Use<RunCommand>();
            For<RootCommand>().Singleton().Use<RootCommand>();
        }
    }
}
