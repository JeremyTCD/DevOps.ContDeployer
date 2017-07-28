using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.Newtonsoft.Json.Utils;
using JeremyTCD.PipelinesCE.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Xunit;

namespace JeremyTCD.PipelinesCE.ConfigHost.Tests.IntegrationTests
{
    public class ConfigHostStartupIntegrationTests
    {
        /// <summary>
        /// This test ensures that <see cref="ConfigHostStartup.CreateContainer"/> configures services correctly as well
        /// as that <see cref="ConfigHostStartup.Configure(IContainer, PipelinesCEOptions)"/> configures logging correctly.
        /// </summary>
        [Fact]
        public void Main_ConfiguresServicesAndRunsPipeline()
        {
            // Arrange
            string testPipeline = "Stub";
            PipelinesCEOptions stubPipelinesCEOptions = new PipelinesCEOptions
            {
                Pipeline = testPipeline,
                Debug = true,
                // TODO write to temp folder for verification
                LogFile = "C:/Users/Jeremy/Documents/Visual Studio 2017/Projects/JeremyTCD.PipelinesCE/test/ConfigHost/test.log"
            };
            SharedPluginOptions stubSharedPluginOptions = new SharedPluginOptions();

            string pipelinesCEOptionsJson = JsonConvert.SerializeObject(stubPipelinesCEOptions, new PrivateFieldsJsonConverter());
            string sharedPluginOptionsJson = JsonConvert.SerializeObject(stubSharedPluginOptions, new PrivateFieldsJsonConverter());

            ThreadSpecificStringWriter tssw = new ThreadSpecificStringWriter();
            Console.SetOut(tssw);

            // Act
            ConfigHostStartup.Main(new string[] { pipelinesCEOptionsJson, sharedPluginOptionsJson });

            // Assert
            tssw.Dispose();
            string result = tssw.ToString();
            // TODO verify from console or log file
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
                ProjectFile = testProject
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
            Assert.Equal(PipelinesCEOptions.DefaultProjectFile, resultPipelinesCEOptions.ProjectFile);
            Assert.Equal(string.Format(Strings.Log_ExecutableAndProjectVersionsDoNotMatch,
                $"{Environment.NewLine}{ConfigHostStartup.NormalizeFieldName(pipelinesCEOptionsExtraFieldKey)}{Environment.NewLine}{ConfigHostStartup.NormalizeFieldName(sharedPluginOptionsExtraFieldKey)}",
                $"{Environment.NewLine}{ConfigHostStartup.NormalizeFieldName(projectFieldKey)}{Environment.NewLine}{ConfigHostStartup.NormalizeFieldName(dryRunFieldKey)}"),
                warnings);
        }

        // TODO test logconfig.configure
        //[Theory]
        //[MemberData(nameof(ConfiguresLoggerFactoryData))]
        //public void Configure_ConfiguresLoggerFactoryCorrectly(LogLevel minLogLevel, bool verbose)
        //{
        //    // Arrange
        //    IContainer container = new Container(r =>
        //    {
        //        r.For<ILoggerFactory>().Singleton().Use<LoggerFactory>();
        //    });

        //    PipelinesCEOptions pipelineOptions = new PipelinesCEOptions
        //    {
        //        Verbose = verbose
        //    };

        //    // Act
        //    ConfigHostStartup.Configure(container, pipelineOptions);

        //    // Assert
        //    ILoggerFactory loggerFactory = container.GetInstance<ILoggerFactory>();
        //    ILogger logger = loggerFactory.CreateLogger("");
        //    Assert.True(logger.IsEnabled(minLogLevel) && (minLogLevel == LogLevel.Trace || !logger.IsEnabled(minLogLevel - 1)));
        //}

        //public static IEnumerable<object[]> ConfiguresLoggerFactoryData()
        //{
        //    yield return new object[] { PipelinesCEOptions.DebugMinLogLevel, true };
        //    yield return new object[] { PipelinesCEOptions.NormalMinLogLevel, false };
        //}
    }
}
