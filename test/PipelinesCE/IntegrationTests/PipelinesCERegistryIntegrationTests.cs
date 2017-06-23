using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StructureMap;
using StructureMap.Pipeline;
using StructureMap.Query;
using System;
using System.Linq;
using Xunit;

namespace JeremyTCD.PipelinesCE.Tests.IntegrationTests
{
    /// <summary>
    /// These tests ensure that PipelineCERegistry behaves correctly
    /// </summary>
    public class PipelinesCERegistryIntegrationTests
    {
        [Fact]
        public void PipelinesRegistry_ConfiguresServicesCorrectly()
        {
            // Arrange and Act
            Container container = new Container(new PipelinesCERegistry());

            // Assert
            IModel model = container.Model;
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(ILoggerFactory) && r.ReturnedType == typeof(LoggerFactory)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(ILogger<>) && r.ReturnedType == typeof(Logger<>)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));

            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IOptions<>) && r.ReturnedType == typeof(OptionsManager<>)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IOptionsMonitor<>) && r.ReturnedType == typeof(OptionsMonitor<>)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IOptionsSnapshot<>) && r.ReturnedType == typeof(OptionsSnapshot<>)
                && r.Lifecycle.GetType() == typeof(ContainerLifecycle)));
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IOptionsFactory<>) && r.ReturnedType == typeof(OptionsFactory<>)
                && r.Lifecycle.GetType() == typeof(UniquePerRequestLifecycle))); 
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IOptionsCache<>) && r.ReturnedType == typeof(OptionsCache<>)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));

            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IAssemblyService) && r.ReturnedType == typeof(AssemblyService)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IPathService) && r.ReturnedType == typeof(PathService)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IDirectoryService) && r.ReturnedType == typeof(DirectoryService)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IMSBuildService) && r.ReturnedType == typeof(MSBuildService)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IActivatorService) && r.ReturnedType == typeof(ActivatorService)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(ILoggingService<>) && r.ReturnedType == typeof(LoggingService<>)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IProcessService) && r.ReturnedType == typeof(ProcessService)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));

            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IPipelineRunner) && r.ReturnedType == typeof(PipelineRunner)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IPipelineContext) && r.ReturnedType == typeof(PipelineContext)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IStepContextFactory) && r.ReturnedType == typeof(StepContextFactory)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IPipelineContextFactory) && r.ReturnedType == typeof(PipelineContextFactory)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(PipelinesCE) && r.ReturnedType == typeof(PipelinesCE)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
        }

        [Fact]
        public void PipelinesRegistry_GeneratedContainerDisposesCorrectly()
        {
            // Arrange and Act
            Container container = new Container(new PipelinesCERegistry());
            PipelinesCE pipelinesCE = container.GetInstance<PipelinesCE>();
            ILoggerFactory loggerFactory = container.GetInstance<ILoggerFactory>();
            container.Dispose();

            // Assert
            Assert.True(pipelinesCE.IsDisposed);
            Assert.Throws<ObjectDisposedException>(() => loggerFactory.AddProvider(null));
        }
    }
}
