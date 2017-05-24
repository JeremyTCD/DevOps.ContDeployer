using StructureMap;

namespace JeremyTCD.PipelinesCE.ConsoleApplication
{
    public class Startup
    { 
        public void ConfigureServices(IContainer main)
        {
            main.AddPipelinesCE();
        }
    }
}
