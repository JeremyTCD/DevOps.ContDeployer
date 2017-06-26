using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.PipelinesCE.CommandLineApp.Tests.UnitTests
{
    public class StartupUnitTests
    {
        private MockRepository _mockRepository { get; }

        public StartupUnitTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Theory]
        [MemberData(nameof(ConfiguresLoggerFactoryData))]
        public void Configure_ConfiguresLoggerFactory(LogLevel minLogLevel, string[] args)
        {
            // Arrange
            ILoggerFactory loggerFactory = new LoggerFactory();

            Startup startup = new Startup();

            // Act
            startup.Configure(loggerFactory, args);

            // Assert
            ILogger logger = loggerFactory.CreateLogger("test");
            Assert.True(logger.IsEnabled(minLogLevel) && (minLogLevel == LogLevel.Trace || !logger.IsEnabled(minLogLevel - 1)));
        }

        public static IEnumerable<object[]> ConfiguresLoggerFactoryData()
        {
            CommandLineAppOptions claOptions = new CommandLineAppOptions();

            yield return new object[] { claOptions.VerboseMinLogLevel, new string[] { $"--{Strings.OptionLongName_Verbose}"} };
            yield return new object[] { claOptions.VerboseMinLogLevel, new string[] { $"-{Strings.OptionShortName_Verbose}" } };
            yield return new object[] { claOptions.DefaultMinLogLevel, new string[] { } };
        }
    }
}
