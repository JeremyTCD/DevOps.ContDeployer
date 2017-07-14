using Newtonsoft.Json;
using System;
using Xunit;

namespace JeremyTCD.PipelinesCE.PluginAndConfigTools.Tests.IntegrationTests
{
    public class PrivateFieldsJsonConverterIntegrationTests
    {
        [Fact]
        public void Serialize_SerializesPrivateFields()
        {
            // Arrange
            int testField1Value = 1;
            string testField2Value = "testField2Value";
            bool? testField3Value = true;

            StubClass stubClass = new StubClass
            {
                StubProperty1 = testField1Value,
                StubProperty2 = testField2Value,
                StubProperty3 = testField3Value
            };

            // Act
            string result = JsonConvert.SerializeObject(stubClass, new PrivateFieldsJsonConverter());

            // Assert
            //Assert.Contains("\"_stubField1", result);
        }

        private class StubClass
        {
            private int _stubField1;
            private string _stubField2;
            private bool? _stubField3;
            public int _stubField4;

            public int StubProperty1
            {
                get
                {
                    return _stubField1;
                }
                set
                {
                    _stubField1 = value;
                }
            }

            public string StubProperty2
            {
                get
                {
                    return _stubField2;
                }
                set
                {
                    _stubField2 = value;
                }
            }

            public bool? StubProperty3
            {
                get
                {
                    return _stubField3;
                }
                set
                {
                    _stubField3 = value;
                }
            }
        }

        //[Fact]
        //public void ParseArgs_DeserializesAndLogsIfArgsAreFromAPipelineOptionsVersionWithFewerMembers()
        //{
        //    // Arrange
        //    string testPipeline = "testPipeline";
        //    string testProject = "testProject";
        //    bool dryRun = true;

        //    PipelineOptions stubPipelineOptions = new PipelineOptions
        //    {
        //        Pipeline = testPipeline,
        //        Project = testProject,
        //        DryRun = dryRun
        //    };

        //    string json = JsonConvert.SerializeObject(stubPipelineOptions, new JsonSerializerSettings { ContractResolver = new PipelineOptionsContractResolver() });
        //    JObject obj = JObject.Parse(json);
        //    obj.Property("_pipeline").Remove();
        //    json = obj.ToString();

        //    // Act
        //    PipelineOptions result = Program.ParseArgs(new string[] { json });

        //    // Assert
        //    Assert.Equal(PipelineOptions.DefaultPipeline, result.Pipeline);
        //    Assert.Equal(testProject, result.Project);
        //    Assert.Equal(dryRun, result.DryRun);
        //}

        //// TODO same members but different member types?
        //[Fact]
        //public void ParseArgs_Deserializes()
        //{
        //    // Arrange
        //    string testPipeline = "testPipeline";
        //    PipelineOptions stubPipelineOptions = new PipelineOptions
        //    {
        //        Pipeline = testPipeline
        //    };

        //    string json = JsonConvert.SerializeObject(stubPipelineOptions, new JsonSerializerSettings { ContractResolver = new PipelineOptionsContractResolver() });

        //    // Act
        //    PipelineOptions result = Program.ParseArgs(new string[] { json });

        //    // Assert
        //    Assert.Equal(PipelineOptions.DefaultDryRun, result.DryRun);
        //    Assert.Equal(testPipeline, result.Pipeline);
        //    Assert.Equal(PipelineOptions.DefaultProject, result.Project);
        //}
    }
}
