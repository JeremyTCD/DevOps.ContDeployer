using JeremyTCD.ContDeployer.PluginTools;
using System;
using Xunit;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace PluginTools.Tests
{
    public class PluginBaseTests
    {
        [Fact]
        public void CombineConfigs_CombinesConfigs()
        {
            // Arrange
            Dictionary<string, object> primaryConfig = new Dictionary<string, object>()
            {
                {"key1", "primaryValue1" },
                {"key3", "primaryValue2" }
            };
            Dictionary<string, object> secondaryConfig = new Dictionary<string, object>()
            {
                {"key1", "secondaryValue1" },
                {"key2", "secondaryValue2" }
            };

            TestPlugin testPlugin = new TestPlugin();

            // Act
            IDictionary<string, object> result = testPlugin.CombineConfigs(primaryConfig, secondaryConfig);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(primaryConfig["key1"], result["key1"]);
            Assert.Equal(secondaryConfig["key2"], result["key2"]);
            Assert.Equal(primaryConfig["key3"], result["key3"]);
        }

        public class TestPlugin : PluginBase
        {
            public override IDictionary<string, object> DefaultConfig { get; set; }

            public override void Run(IDictionary<string, object> config, PipelineContext context, ILogger logger, LinkedList<PipelineStep> steps)
            {
                throw new NotImplementedException();
            }
        }
    }
}
