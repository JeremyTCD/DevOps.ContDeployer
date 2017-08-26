using JeremyTCD.PipelinesCE.Config;

namespace PipelinesCEConfig
{
    public class Program
    {
        public static int Main(string[] args)
        {
            ConfigProgram host = new ConfigProgram(); // TODO provide a builder instead 

            return host.Run(args);
        }
    }
}
