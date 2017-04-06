using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
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
            IDictionary<string, object> testPluginConfig = new Dictionary<string, object>();
            string testPlugin1Name = "testPlugin1";
            string testPlugin2Name = "testPlugin2";
            string testPlugin3Name = "testPlugin3";
            PipelineOptions options = new PipelineOptions()
            {
                PipelineSteps = new List<PipelineStep> {
                        new PipelineStep()
                        {
                            PluginName = testPlugin1Name,
                            Config = testPluginConfig
                        }, new PipelineStep()
                        {
                            PluginName = testPlugin2Name,
                            Config = testPluginConfig
                        }, new PipelineStep()
                        {
                            PluginName = testPlugin3Name,
                            Config = testPluginConfig
                        }
                    }
            };

            Mock<IOptions<PipelineOptions>> mockOptionsAccessor = new Mock<IOptions<PipelineOptions>>();
            mockOptionsAccessor.
                Setup(o => o.Value).
                Returns(options);

            Mock<IPlugin> mockPlugin = new Mock<IPlugin>();
            IPlugin mockPluginObject = mockPlugin.Object;

            Mock<IDictionary<string, IPlugin>> mockPlugins = new Mock<IDictionary<string, IPlugin>>();
            mockPlugins.
                Setup(p => p.TryGetValue(It.Is<string>(s => s == testPlugin1Name), out mockPluginObject)).
                Returns(true);
            mockPlugins.
                Setup(p => p.TryGetValue(It.Is<string>(s => s == testPlugin2Name), out mockPluginObject)).
                Returns(true);
            mockPlugins.
                Setup(p => p.TryGetValue(It.Is<string>(s => s == testPlugin3Name), out mockPluginObject)).
                Returns(true);

            Mock<IRepository> mockRepository = new Mock<IRepository>();

            PipelineContext context = new PipelineContext(mockPlugins.Object, mockRepository.Object);

            mockPlugin.
                Setup(m => m.Run(It.Is<IDictionary<string, object>>(c => c == testPluginConfig),
                        It.Is<PipelineContext>(p => p == context),
                        It.Is<LinkedList<PipelineStep>>(l => l.Count == 2))
                    );
            mockPlugin.
                Setup(m => m.Run(It.Is<IDictionary<string, object>>(c => c == testPluginConfig),
                        It.Is<PipelineContext>(p => p == context),
                        It.Is<LinkedList<PipelineStep>>(l => l.Count == 1))
                    );
            mockPlugin.
                Setup(m => m.Run(It.Is<IDictionary<string, object>>(c => c == testPluginConfig),
                        It.Is<PipelineContext>(p => p == context),
                        It.Is<LinkedList<PipelineStep>>(l => l.Count == 0))
                    );

            Mock<ILogger<Pipeline>> mockLogger = new Mock<ILogger<Pipeline>>();

            Pipeline pipeline = new Pipeline(context, mockOptionsAccessor.Object, mockLogger.Object);

            // Act
            pipeline.Run();

            // Assert
            mockOptionsAccessor.VerifyAll();
            mockPlugins.VerifyAll();
            mockPlugin.VerifyAll();
        }
    }
}