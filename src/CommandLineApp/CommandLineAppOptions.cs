using Microsoft.Extensions.Logging;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    public class CommandLineAppOptions
    {
        public LogLevel DefaultMinLogLevel { get; } = LogLevel.Information;
        public LogLevel VerboseMinLogLevel { get; } = LogLevel.Debug;
        public string LogFileFormat { get; } = "log-{Date}.txt"; // https://github.com/serilog/serilog-extensions-logging-file
    }
}
