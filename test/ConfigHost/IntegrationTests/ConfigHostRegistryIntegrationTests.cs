using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StructureMap;
using StructureMap.Pipeline;
using StructureMap.Query;
using System;
using System.Linq;
using Xunit;

namespace JeremyTCD.PipelinesCE.ConfigHost.Tests.IntegrationTests
{
    public class ConfigHostRegistryIntegrationTests
    {
        [Fact]
        public void PipelinesRegistry_ConfiguresServicesCorrectly()
        {
            // Arrange and Act
            Container container = new Container(new ConfigHostRegistry());

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
            //Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IOptionsFactory<>) && r.ReturnedType == typeof(OptionsFactory<>)
            //    && r.Lifecycle.GetType() == typeof(UniquePerRequestLifecycle))); 
            //Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IOptionsCache<>) && r.ReturnedType == typeof(OptionsCache<>)
            //    && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));

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
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IDependencyContextService) && r.ReturnedType == typeof(DependencyContextService)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IFileService) && r.ReturnedType == typeof(FileService)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));

            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(ConfigHostCore) && r.ReturnedType == typeof(ConfigHostCore)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IPipelineRunner) && r.ReturnedType == typeof(PipelineRunner)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IPipelineLoader) && r.ReturnedType == typeof(PipelineLoader)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IPipelineContextFactory) && r.ReturnedType == typeof(PipelineContextFactory)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(IStepContextFactory) && r.ReturnedType == typeof(StepContextFactory)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
        }

        [Fact]
        public void PipelinesRegistry_GeneratedContainerDisposesCorrectly()
        {
            // Arrange and Act
            Container container = new Container(new ConfigHostRegistry());
            PipelineLoader loader = container.GetInstance<IPipelineLoader>() as PipelineLoader;
            ILoggerFactory loggerFactory = container.GetInstance<ILoggerFactory>();
            container.Dispose();

            // Assert
            Assert.True(loader.IsDisposed);
            Assert.Throws<ObjectDisposedException>(() => loggerFactory.AddProvider(null));
        }
    }
}
