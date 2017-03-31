using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.DevOps.ContDeployer.Tests
{
    public class PipelineFactoryTests
    {
        [Fact]
        public void CreatePipeline_CreatesPipeline()
        {
            // Arrange
            PipelineFactoryOptions options = new PipelineFactoryOptions()
            {
                PluginConfigs = new List<PluginConfig>
                {
                    new PluginConfig
                    {
                        Name = nameof(MetadataFactoryPlugin)
                    },
                    new PluginConfig
                    {
                        Name = nameof(TagGeneratorPlugin)
                    },
                    new PluginConfig{
                        Name = nameof(AppVeyorPublisherPlugin)
                    }
                }
            };
            Mock<IOptions<PipelineFactoryOptions>> mockOptionsAccessor = new Mock<IOptions<PipelineFactoryOptions>>();
            mockOptionsAccessor.
                Setup(m => m.Value).
                Returns(options);
            PipelineFactory pipelineFactory = new PipelineFactory(mockOptionsAccessor.Object);

            // Act
            Pipeline result = pipelineFactory.CreatePipeline();

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Plugins);
            Assert.Equal(result.Plugins[0].GetType().Name, nameof(MetadataFactoryPlugin));
            Assert.Equal(result.Plugins[1].GetType().Name, nameof(TagGeneratorPlugin));
            Assert.Equal(result.Plugins[2].GetType().Name, nameof(AppVeyorPublisherPlugin));
            Assert.Equal(result.Configs, options.PluginConfigs);
        }

        [Fact]
        public void CreatePipeline_ThrowsExceptionIfARequestedPluginDoesNotExist()
        {
            // Arrange
            PipelineFactoryOptions options = new PipelineFactoryOptions()
            {
                PluginConfigs = new List<PluginConfig>
                {
                    new PluginConfig
                    {
                        Name = "NonExistantPlugin"
                    }
                }
            };
            Mock<IOptions<PipelineFactoryOptions>> mockOptionsAccessor = new Mock<IOptions<PipelineFactoryOptions>>();
            mockOptionsAccessor.
                Setup(m => m.Value).
                Returns(options);
            PipelineFactory pipelineFactory = new PipelineFactory(mockOptionsAccessor.Object);

            // Act and Assert
            Assert.Throws<Exception>(() => pipelineFactory.CreatePipeline());
        }
    }
}