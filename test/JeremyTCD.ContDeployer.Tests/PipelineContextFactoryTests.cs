using JeremyTCD.ContDeployer.Plugin.AppVeyorPublisher;
using JeremyTCD.ContDeployer.Plugin.LogMetadataFactory;
using JeremyTCD.ContDeployer.Plugin.TagGenerator;
using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.DotNetCore.Utils;
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
        public string TempDir { get; set; }
        public string TempPluginsDir { get; set; }

        public PipelineContextFactoryTests()
        {
            // Reset test folder
            TempDir = Path.Combine(Path.GetTempPath(), "ContDeployerTemp");
            TempPluginsDir = Path.Combine(TempDir, "plugins");
            if (Directory.Exists(TempDir))
            {
                Directory.Delete(TempDir, true);
            }
            Directory.CreateDirectory(TempPluginsDir);
        }

        [Fact]
        public void Build_CreatesPipelineContext()
        {
            // Arrange
            // Copy TestPlugin assembly over to temp plugins dir
            string currentDir = typeof(PipelineContextFactoryTests).GetTypeInfo().Assembly.Location;
            // Can't reference JeremyTCD.TestPlugin 
            string testPluginAssemblyDir = Path.Combine(currentDir, "../../../../../JeremyTCD.ContDeployer.TestPlugin/bin/Debug/netcoreapp1.1");
            string[] files = Directory.GetFiles(testPluginAssemblyDir);
            foreach (string file in files)
            {
                File.Copy(file, Path.Combine(TempPluginsDir, Path.GetFileName(file)), true);
            }
            // Set current directory to the temp folder
            Directory.SetCurrentDirectory(TempDir);
            
            Mock<ILogger<PipelineContextFactory>> mockLogger = new Mock<ILogger<PipelineContextFactory>>();
            AssemblyService assemblyService = new AssemblyService(); 
            PipelineContextFactory pipelineContextFactory = new PipelineContextFactory(mockLogger.Object, assemblyService);

            // Act
            PipelineContext result = pipelineContextFactory.Build();

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.GlobalData);
            Assert.NotNull(result.Plugins);
            Assert.Equal(4, result.Plugins.Count);
            Assert.True(result.Plugins.Keys.Contains(nameof(LogMetadataFactory)));
            Assert.True(result.Plugins.Keys.Contains(nameof(TagGenerator)));
            Assert.True(result.Plugins.Keys.Contains(nameof(AppVeyorPublisher)));
            // Can't reference JeremyTCD.TestPlugin
            Assert.True(result.Plugins.Keys.Contains("TestPlugin"));
        }
    }
}