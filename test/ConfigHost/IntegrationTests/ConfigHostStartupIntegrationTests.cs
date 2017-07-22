using JeremyTCD.Newtonsoft.Json.Utils;
using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StructureMap;
using System;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.PipelinesCE.ConfigHost.Tests.IntegrationTests
{
    public class ConfigHostStartupIntegrationTests
    {
        /// <summary>
        /// This test ensures that <see cref="ConfigHostRegistry"/> configures services correctly.
        /// </summary>
        [Fact]
        public void Main_RunsPipeline()
        {
            // Arrange
            string testPipeline = "Stub";
            PipelineOptions stubOptions = new PipelineOptions
            {
                Pipeline = testPipeline
            };

            string json = JsonConvert.SerializeObject(stubOptions, new PrivateFieldsJsonConverter());

            // Act
            int exitCode = ConfigHostStartup.Main(new string[] { json });

            // Assert
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public void ParseArgs_ParsesArgsAndLogsMissingAndExtraFields()
        {
            // Arrange
            string testPipeline = "testPipeline";
            string testProject = "testProject";
            string extraFieldKey = "_extraFieldKey";
            string extraFieldValue = "extraFieldValue";
            string projectFieldKey = "_project";
            bool testDryRun = true;

            PipelineOptions stubOptions = new PipelineOptions
            {
                DryRun = true,
                Pipeline = testPipeline,
                Project = testProject
            };

            string json = JsonConvert.SerializeObject(stubOptions, new PrivateFieldsJsonConverter());
            JObject jObject = JsonConvert.DeserializeObject<JObject>(json);
            jObject.Add(extraFieldKey, extraFieldValue);
            jObject.Remove(projectFieldKey);
            json = jObject.ToString();

            ConfigHostStartup program = new ConfigHostStartup();

            // Act
            (PipelineOptions options, string warnings) = ConfigHostStartup.ParseArgs(new string[] { json });

            // Assert
            Assert.Equal(testDryRun, options.DryRun);
            Assert.Equal(testPipeline, options.Pipeline);
            Assert.Equal(PipelineOptions.DefaultVerbose, options.Verbose);
            Assert.Equal(PipelineOptions.DefaultProject, options.Project);
            Assert.Equal(string.Format(Strings.Log_ExecutableAndProjectVersionsDoNotMatch,
                Environment.NewLine + ConfigHostStartup.NormalizeFieldName(extraFieldKey),
                Environment.NewLine + ConfigHostStartup.NormalizeFieldName(projectFieldKey)),
                warnings);
        }

        [Theory]
        [MemberData(nameof(ConfiguresLoggerFactoryData))]
        public void Configure_ConfiguresLoggerFactoryCorrectly(LogLevel minLogLevel, bool verbose)
        {
            // Arrange
            IContainer container = new Container(r =>
            {
                r.For<ILoggerFactory>().Singleton().Use<LoggerFactory>();
            });

            PipelineOptions pipelineOptions = new PipelineOptions
            {
                Verbose = verbose
            };

            // Act
            ConfigHostStartup.Configure(container, pipelineOptions);

            // Assert
            ILoggerFactory loggerFactory = container.GetInstance<ILoggerFactory>();
            ILogger logger = loggerFactory.CreateLogger("");
            Assert.True(logger.IsEnabled(minLogLevel) && (minLogLevel == LogLevel.Trace || !logger.IsEnabled(minLogLevel - 1)));
        }

        public static IEnumerable<object[]> ConfiguresLoggerFactoryData()
        {
            yield return new object[] { PipelineOptions.VerboseMinLogLevel, true };
            yield return new object[] { PipelineOptions.DefaultMinLogLevel, false };
        }
    }
}
