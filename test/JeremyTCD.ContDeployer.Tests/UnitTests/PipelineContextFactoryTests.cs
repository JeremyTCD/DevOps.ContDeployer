using JeremyTCD.ContDeployer.Plugin.AppVeyorPublisher;
using JeremyTCD.ContDeployer.Plugin.LogMetadataFactory;
using JeremyTCD.ContDeployer.Plugin.TagGenerator;
using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.DotNetCore.Utils;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit;

namespace JeremyTCD.ContDeployer.Tests
{
    public class PipelineContextFactoryTests
    {
        [Fact]
        public void Build_CreatesPipelineContext()
        {
            // Arrange
            // Copy TestPlugin assembly over to temp plugins dir
            Mock<ILogger<PipelineContextFactory>> mockLogger = new Mock<ILogger<PipelineContextFactory>>();
            Mock<IAssemblyService> mockAssemblyService = new Mock<IAssemblyService>();
            mockAssemblyService.
                Setup(m => m.GetReferencingAssemblies(It.Is<string>(s => s == typeof(IPlugin).GetTypeInfo().Assembly.GetName().Name))).
                Returns(new Assembly[] { typeof(LogMetadataFactory).GetTypeInfo().Assembly,
                     typeof(TagGenerator).GetTypeInfo().Assembly});
            mockAssemblyService.
                Setup(m => m.GetAssembliesInDir(It.Is<string>(s => s == Path.Combine(Directory.GetCurrentDirectory(), "plugins")), 
                    It.Is<bool>(b => b))).
                Returns(new Assembly[] {
                     typeof(AppVeyorPublisher).GetTypeInfo().Assembly
                });
            Mock<IRepository> mockRepository = new Mock<IRepository>();

            PipelineContextFactory pipelineContextFactory = new PipelineContextFactory(mockLogger.Object, 
                mockAssemblyService.Object,
                mockRepository.Object);

            // Act
            PipelineContext result = pipelineContextFactory.Build();

            // Assert
            mockAssemblyService.VerifyAll();
            Assert.NotNull(result);
            Assert.NotNull(result.GlobalData);
            Assert.NotNull(result.Plugins);
            Assert.NotNull(result.Repository);
            Assert.Equal(3, result.Plugins.Count);
            Assert.True(result.Plugins.Keys.Contains(nameof(LogMetadataFactory)));
            Assert.True(result.Plugins.Keys.Contains(nameof(TagGenerator)));
            Assert.True(result.Plugins.Keys.Contains(nameof(AppVeyorPublisher)));
        }
    }
}