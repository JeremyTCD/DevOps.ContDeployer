using JeremyTCD.Newtonsoft.Json.Utils;
using JeremyTCD.PipelinesCE.Core;
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
            PipelinesCEOptions stubPipelinesCEOptions = new PipelinesCEOptions
            {
                Pipeline = testPipeline
            };
            SharedPluginOptions stubSharedPluginOptions = new SharedPluginOptions();

            string pipelinesCEOptionsJson = JsonConvert.SerializeObject(stubPipelinesCEOptions, new PrivateFieldsJsonConverter());
            string sharedPluginOptionsJson = JsonConvert.SerializeObject(stubSharedPluginOptions, new PrivateFieldsJsonConverter());

            // Act
            int exitCode = ConfigHostStartup.Main(new string[] { pipelinesCEOptionsJson, sharedPluginOptionsJson });

            // Assert
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public void ParseArgs_ParsesArgsAndLogsMissingAndExtraFields()
        {
            // Arrange
            string testPipeline = "testPipeline";
            string testProject = "testProject";
            string pipelinesCEOptionsExtraFieldKey = "_pipelinesCEOptionsExtraFieldKey";
            string sharedPluginOptionsExtraFieldKey = "_sharedPluginOptionsExtraFieldKey";
            string extraFieldValue = "extraFieldValue";
            string projectFieldKey = "_project";
            string dryRunFieldKey = "_dryRun";
            bool testDryRun = true;

            PipelinesCEOptions stubPipelinesCEOptions = new PipelinesCEOptions
            {
                Pipeline = testPipeline,
                Project = testProject
            };

            SharedPluginOptions stubSharedPluginOptions = new SharedPluginOptions
            {
                DryRun = testDryRun
            };

            string pipelinesCEOptionsJson = JsonConvert.SerializeObject(stubPipelinesCEOptions, new PrivateFieldsJsonConverter());
            JObject jObject = JsonConvert.DeserializeObject<JObject>(pipelinesCEOptionsJson);
            jObject.Add(pipelinesCEOptionsExtraFieldKey, extraFieldValue);
            jObject.Remove(projectFieldKey);
            pipelinesCEOptionsJson = jObject.ToString();

            string sharedPluginOptionsJson = JsonConvert.SerializeObject(stubSharedPluginOptions, new PrivateFieldsJsonConverter());
            jObject = JsonConvert.DeserializeObject<JObject>(sharedPluginOptionsJson);
            jObject.Add(sharedPluginOptionsExtraFieldKey, extraFieldValue);
            jObject.Remove(dryRunFieldKey);
            sharedPluginOptionsJson = jObject.ToString();

            ConfigHostStartup program = new ConfigHostStartup();

            // Act
            (PipelinesCEOptions resultPipelinesCEOptions, SharedPluginOptions resultSharedPluginOptions, string warnings) = ConfigHostStartup.
                ParseArgs(new string[] { pipelinesCEOptionsJson, sharedPluginOptionsJson});

            // TODO warnings are wrong
            // Assert
            Assert.Equal(testDryRun, stubSharedPluginOptions.DryRun);
            Assert.Equal(testPipeline, resultPipelinesCEOptions.Pipeline);
            Assert.Equal(PipelinesCEOptions.DefaultVerbose, resultPipelinesCEOptions.Verbose);
            Assert.Equal(PipelinesCEOptions.DefaultProject, resultPipelinesCEOptions.Project);
            Assert.Equal(string.Format(Strings.Log_ExecutableAndProjectVersionsDoNotMatch,
                $"{Environment.NewLine}{ConfigHostStartup.NormalizeFieldName(pipelinesCEOptionsExtraFieldKey)}{Environment.NewLine}{ConfigHostStartup.NormalizeFieldName(sharedPluginOptionsExtraFieldKey)}",
                $"{Environment.NewLine}{ConfigHostStartup.NormalizeFieldName(projectFieldKey)}{Environment.NewLine}{ConfigHostStartup.NormalizeFieldName(dryRunFieldKey)}"),
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

            PipelinesCEOptions pipelineOptions = new PipelinesCEOptions
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
            yield return new object[] { PipelinesCEOptions.VerboseMinLogLevel, true };
            yield return new object[] { PipelinesCEOptions.DefaultMinLogLevel, false };
        }
    }
}
