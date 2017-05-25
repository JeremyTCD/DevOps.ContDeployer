using StructureMap;

namespace JeremyTCD.PipelinesCE.ConsoleApplication
{
    public class ConsoleApplication
    {
        public static void Main(string[] args)
        {
            Startup startup = new Startup();
            IContainer main = new Container();
            startup.ConfigureServices(main);

            PipelinesCE pipelinesCE = main.GetInstance<PipelinesCE>();

            // TODO get options from command line arguments
            pipelinesCE.Run(null);
        }
    }
}
