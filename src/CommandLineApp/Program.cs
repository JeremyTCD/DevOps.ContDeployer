using StructureMap;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    public class Program
    {
        public static int Main(string[] args)
        {
            IContainer container = null;

            try
            {
                // Initialize container
                container = new Container(new CommandLineAppRegistry());

                RootCommand rootCommand = container.GetInstance<RootCommand>();
                return rootCommand.Execute(args);
            }
            finally
            {
                if (container != null)
                {
                    container.Dispose(); // Impossible to know what resources services may use
                }
            }
        }
    }
}
