using StructureMap;
using System.Linq;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    public class CommandLineApp
    {
        public static void Main(string[] args)
        {
            Startup startup = new Startup();
            IContainer mainContainer = new Container();
            startup.ConfigureServices(mainContainer);

            DefaultCommand defaultCommand = mainContainer.GetInstance<DefaultCommand>();
            args = args.Select(s => s.ToLowerInvariant()).ToArray();
            defaultCommand.Execute(args);
        }
    }
}
 