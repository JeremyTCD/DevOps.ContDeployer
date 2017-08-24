using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.Newtonsoft.Json.Utils;
using JeremyTCD.PipelinesCE.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Xunit;

namespace JeremyTCD.PipelinesCE.Config.Tests.IntegrationTests
{
    public class ConfigProgramIntegrationTests
    {
        /// <summary>
        /// This test ensures that <see cref="ConfigProgram.CreateContainer"/> configures services correctly as well
        /// as that <see cref="ConfigProgram.Configure(IContainer, PipelinesCEOptions)"/> configures logging correctly.
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
            SharedStepOptions stubSharedPluginOptions = new SharedStepOptions();

            string pipelinesCEOptionsJson = JsonConvert.SerializeObject(stubPipelinesCEOptions, new PrivateFieldsJsonConverter());
            string sharedPluginOptionsJson = JsonConvert.SerializeObject(stubSharedPluginOptions, new PrivateFieldsJsonConverter());

            ThreadSpecificStringWriter tssw = new ThreadSpecificStringWriter();
            Console.SetOut(tssw);

            // Act
            //ConfigHost.Main(new string[] { pipelinesCEOptionsJson, sharedPluginOptionsJson });

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
            string projectFileFieldKey = "_projectFile";
            string dryRunFieldKey = "_dryRun";
            bool testDryRun = true;

            PipelinesCEOptions stubPipelinesCEOptions = new PipelinesCEOptions
            {
                // Arbitrarily chosen properties 
                Pipeline = testPipeline,
                ProjectFile = testProject
            };

            SharedStepOptions stubSharedPluginOptions = new SharedStepOptions
            {
                // Arbitrarily chosen properties
                DryRun = testDryRun
            };

            string pipelinesCEOptionsJson = JsonConvert.SerializeObject(stubPipelinesCEOptions, new PrivateFieldsJsonConverter());
            JObject jObject = JsonConvert.DeserializeObject<JObject>(pipelinesCEOptionsJson);
            jObject.Add(pipelinesCEOptionsExtraFieldKey, extraFieldValue);
            jObject.Remove(projectFileFieldKey);
            pipelinesCEOptionsJson = jObject.ToString();

            string sharedPluginOptionsJson = JsonConvert.SerializeObject(stubSharedPluginOptions, new PrivateFieldsJsonConverter());
            jObject = JsonConvert.DeserializeObject<JObject>(sharedPluginOptionsJson);
            jObject.Add(sharedPluginOptionsExtraFieldKey, extraFieldValue);
            jObject.Remove(dryRunFieldKey);
            sharedPluginOptionsJson = jObject.ToString();

            // Act
            (PipelinesCEOptions resultPipelinesCEOptions, SharedStepOptions resultSharedPluginOptions, string warnings) = ConfigProgram.
                ParseArgs(new string[] { pipelinesCEOptionsJson, sharedPluginOptionsJson});

            // TODO warnings are wrong
            // Assert
            Assert.Equal(testDryRun, stubSharedPluginOptions.DryRun);
            Assert.Equal(testPipeline, resultPipelinesCEOptions.Pipeline);
            Assert.Equal(PipelinesCEOptions.DefaultVerbose, resultPipelinesCEOptions.Verbose);
            Assert.Equal(PipelinesCEOptions.DefaultProjectFile, resultPipelinesCEOptions.ProjectFile);
            Assert.Equal(string.Format(Strings.Log_ExecutableAndProjectVersionsDoNotMatch,
                $"{Environment.NewLine}{ConfigProgram.NormalizeFieldName(pipelinesCEOptionsExtraFieldKey)}{Environment.NewLine}{ConfigProgram.NormalizeFieldName(sharedPluginOptionsExtraFieldKey)}",
                $"{Environment.NewLine}{ConfigProgram.NormalizeFieldName(projectFileFieldKey)}{Environment.NewLine}{ConfigProgram.NormalizeFieldName(dryRunFieldKey)}"),
                warnings);
        }
    }
}
