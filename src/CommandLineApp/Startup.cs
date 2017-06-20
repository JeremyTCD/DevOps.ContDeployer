using Microsoft.Extensions.Logging;
using System.Linq;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    public class Startup
    {
        public void Configure(ILoggerFactory loggerFactory, string[] args)
        {
            bool verbose = args.Where(s => s == $"--{Strings.OptionLongName_Verbose}" ||
                s == $"-{Strings.OptionShortName_Verbose}").Count() > 0;

            // If need be, claOptions should be made configurable via json (using microsoft.extensions.configuration)
            CommandLineAppOptions claOptions = new CommandLineAppOptions();
            LogLevel logLevel = verbose ? claOptions.VerboseMinLogLevel : claOptions.DefaultMinLogLevel;

            loggerFactory.
                AddConsole(logLevel).
                AddFile(claOptions.LogFileFormat, logLevel).
                AddDebug(logLevel);
        }
    }
}
