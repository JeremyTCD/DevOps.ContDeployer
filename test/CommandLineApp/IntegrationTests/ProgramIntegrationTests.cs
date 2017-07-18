using Microsoft.Extensions.Logging;
using Moq;
using StructureMap;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.PipelinesCE.CommandLineApp.Tests.IntegrationTests
{
    public class ProgramIntegrationTests
    {
        private MockRepository _mockRepository { get; }

        public ProgramIntegrationTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Theory]
        [MemberData(nameof(ConfiguresLoggerFactoryData))]
        public void Configure_ConfiguresLoggerFactoryCorrectly(LogLevel minLogLevel, string[] args)
        {
            // Arrange
            IContainer container = new Container(r =>
            {
                r.For<ILoggerFactory>().Singleton().Use<LoggerFactory>();
            });

            // Act
            Program.Configure(container, args);

            // Assert
            ILoggerFactory loggerFactory = container.GetInstance<ILoggerFactory>();
            ILogger logger = loggerFactory.CreateLogger("");
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
