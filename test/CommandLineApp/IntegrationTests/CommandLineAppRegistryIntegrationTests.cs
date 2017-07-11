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

namespace JeremyTCD.PipelinesCE.CommandLineApp.Tests.IntegrationTests
{
    /// <summary>
    /// These tests ensure that CommandLineAppRegistry behaves correctly
    /// </summary>
    public class CommandLineAppRegistryIntegrationTests
    {
        [Fact]
        public void CommandLineAppRegistry_ConfiguresServicesCorrectly()
        {
            // Arrange and Act
            Container container = new Container(new CommandLineAppRegistry());

            // Assert
            IModel model = container.Model;

            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(ICommandLineUtilsService) && r.ReturnedType == typeof(CommandLineUtilsService)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));

            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(RunCommand) && r.ReturnedType == typeof(RunCommand)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
            Assert.True(model.AllInstances.Any(r => r.PluginType == typeof(RootCommand) && r.ReturnedType == typeof(RootCommand)
                && r.Lifecycle.GetType() == typeof(SingletonLifecycle)));
        }

        [Fact]
        public void CommandLineAppRegistry_GeneratedContainerDisposesCorrectly()
        {
            // Arrange and Act
            Container container = new Container(new CommandLineAppRegistry());
            PipelinesCE pipelinesCE = container.GetInstance<PipelinesCE>();
            ILoggerFactory loggerFactory = container.GetInstance<ILoggerFactory>();
            container.Dispose();

            // Assert
            Assert.True(pipelinesCE.IsDisposed);
            Assert.Throws<ObjectDisposedException>(() => loggerFactory.AddProvider(null));
        }

        [Fact]
        public void CommandLineAppRegistry_GeneratedContainerServicesCanBeOverridenByInstantiatedInstance()
        {
            // Arrange 
            Container container = new Container(new CommandLineAppRegistry());
            PipelinesCE instance = new PipelinesCE(null, null, null, null, null, null, null, null, null);

            // Act
            container.Configure(r => r.For<PipelinesCE>().Use(instance).Singleton());
            PipelinesCE result = container.GetInstance<PipelinesCE>();

            // Assert
            Assert.True(object.ReferenceEquals(instance, result));
        }

        [Fact]
        public void CommandLineAppRegistry_GeneratedContainerServicesCanBeOverridenByNewService()
        {
            // Arrange 
            Container container = new Container(new CommandLineAppRegistry());

            // Act
            container.Configure(r => r.For<PipelinesCE>().Use<StubPipelinesCE>().Singleton());
            PipelinesCE result = container.GetInstance<PipelinesCE>();

            // Assert
            Assert.Equal(typeof(StubPipelinesCE), result.GetType());
        }

        private class StubPipelinesCE : PipelinesCE
        {
            public StubPipelinesCE(IActivatorService activatorService, IDependencyContextService dependencyContextService,
                IAssemblyService assemblyService, 
                IPathService pathService, IDirectoryService directoryService, 
                IMSBuildService msBuildService, IPipelineRunner pipelineRunner, 
                IContainer mainContainer, ILoggingService<PipelinesCE> loggingService) : 
                base(activatorService, dependencyContextService, assemblyService, pathService, directoryService, 
                    msBuildService, pipelineRunner, mainContainer,loggingService)
            {
            }
        }
    }
}
