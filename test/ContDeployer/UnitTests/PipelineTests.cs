using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.ContDeployer.Tests
{
    public class PipelineTests
    {
        [Fact]
        public void Run_RunsThroughSteps()
        {
            // Arrange
            string testPlugin1Name = "testPlugin1";
            string testPlugin2Name = "testPlugin2";
            string testPlugin3Name = "testPlugin3";
            Mock<IConfigurationSection> mockConfigSection = new Mock<IConfigurationSection>();
            Mock<IPluginOptions> mockPluginOptions = new Mock<IPluginOptions>();

            PipelineOptions options = new PipelineOptions()
            {
                PipelineSteps = new List<PipelineStep> {
                        new PipelineStep()
                        {
                            PluginName = testPlugin1Name,
                            Config = mockConfigSection.Object
                        },
                        new PipelineStep(testPlugin2Name , mockPluginOptions.Object),
                        new PipelineStep()
                        {
                            PluginName = testPlugin3Name
                        }
                    }
            };

            Mock<IOptions<PipelineOptions>> mockOptionsAccessor = new Mock<IOptions<PipelineOptions>>();
            mockOptionsAccessor.
                Setup(o => o.Value).
                Returns(options);

            Mock<IPlugin> mockPlugin = new Mock<IPlugin>();
            mockPlugin.
                Setup(m => m.Run(It.IsAny<Dictionary<string,object>>(), It.Is<LinkedList<PipelineStep>>(l => l.Count == 2)));
            mockPlugin.
                Setup(m => m.Run(It.IsAny<Dictionary<string, object>>(), It.Is<LinkedList<PipelineStep>>(l => l.Count == 1)));
            mockPlugin.
                Setup(m => m.Run(It.IsAny<Dictionary<string, object>>(), It.Is<LinkedList<PipelineStep>>(l => l.Count == 0)));

            Mock<IPluginFactory> mockPluginFactory = new Mock<IPluginFactory>();
            mockPluginFactory.
                Setup(p => p.BuildPlugin(It.Is<string>(s => s == testPlugin1Name), 
                    It.Is<IConfigurationSection>(c => c == mockConfigSection.Object))).
                Returns(mockPlugin.Object);
            mockPluginFactory.
                Setup(p => p.BuildPlugin(It.Is<string>(s => s == testPlugin2Name),
                    It.Is<IPluginOptions>(po => po == mockPluginOptions.Object))).
                Returns(mockPlugin.Object);
            mockPluginFactory.
                Setup(p => p.BuildPlugin(It.Is<string>(s => s == testPlugin3Name),
                    It.Is<object>(o => o == null))).
                Returns(mockPlugin.Object);

            Mock<ILogger<Pipeline>> mockLogger = new Mock<ILogger<Pipeline>>();

            Pipeline pipeline = new Pipeline(mockOptionsAccessor.Object, mockLogger.Object, mockPluginFactory.Object);

            // Act
            pipeline.Run();

            // Assert
            mockOptionsAccessor.VerifyAll();
            mockPlugin.VerifyAll();
            mockPluginFactory.VerifyAll();
        }
    }
}