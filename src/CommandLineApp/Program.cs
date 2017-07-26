using Microsoft.Extensions.DependencyInjection;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    public class Program
    {
        public static int Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddCommandLineApp();

            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                RootCommand rootCommand = serviceProvider.GetService<RootCommand>();
                return rootCommand.Execute(args);
            }
        }
    }
}
