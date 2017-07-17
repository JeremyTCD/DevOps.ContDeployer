using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.Newtonsoft.Json.Utils;
using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Xunit;

namespace JeremyTCD.PipelinesCE.PipelineRunner.Tests.IntegrationTests
{
    public class ProgramIntegrationTests
    {
        /// <summary>
        /// This test ensures that <see cref="PipelineRunnerRegistry"/> configures services correctly.
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

            ThreadSpecificStringWriter tssw = new ThreadSpecificStringWriter();
            Console.SetOut(tssw);

            // Act
            Program.Main(new string[] { json });

            // Assert
            tssw.Dispose();
            string output = tssw.ToString();
            Assert.Contains(string.Format(Strings.Log_FinishedRunningPlugin, "StubPlugin"), output);
            Assert.Contains(string.Format(Strings.Log_FinishedRunningPipeline, $"{testPipeline}"), output);
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

            Program program = new Program();

            // Act
            (PipelineOptions options, string warnings) = Program.ParseArgs(new string[] { json });

            // Assert
            Assert.Equal(testDryRun, options.DryRun);
            Assert.Equal(testPipeline, options.Pipeline);
            Assert.Equal(PipelineOptions.DefaultVerbose, options.Verbose);
            Assert.Equal(PipelineOptions.DefaultProject, options.Project);
            Assert.Equal(string.Format(Strings.Log_ExecutableAndProjectVersionsDoNotMatch, 
                Environment.NewLine + Program.NormalizeFieldName(extraFieldKey), 
                Environment.NewLine + Program.NormalizeFieldName(projectFieldKey)), 
                warnings);
        }

        [Fact]
        public void Configure_ConfiguresLoggerFactoryCorrectly()
        {
            // TODO cant mock extensions methods using moq
        }
    }
}
