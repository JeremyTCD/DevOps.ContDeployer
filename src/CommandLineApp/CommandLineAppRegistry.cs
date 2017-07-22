using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.DotNetCore.ProjectHost;
using StructureMap;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    public class CommandLineAppRegistry : Registry
    {
        public CommandLineAppRegistry()
        {
            IncludeRegistry<ProjectHostRegistry>();

            For<ICommandLineUtilsService>().Singleton().Use<CommandLineUtilsService>();

            For<RunCommand>().Singleton().Use<RunCommand>();
            For<RootCommand>().Singleton().Use<RootCommand>();
        }
    }
}
