using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System.Linq;
using Xunit;

namespace JeremyTCD.PipelinesCE.ConfigHost.Tests.IntegrationTests
{
    public class ConfigHostServiceCollectionExtensionsIntegrationTests
    {
        [Fact]
        public void AddConfigHost_ConfiguresServicesCorrectly()
        {
            // Arrange 
            ServiceCollection services = new ServiceCollection();

            // Act
            services.AddConfigHost();

            // Assert
            ServiceDescriptorComparer comparer = new ServiceDescriptorComparer();
            Assert.True(services.Contains(ServiceDescriptor.Singleton<ConfigHostCore, ConfigHostCore>(), comparer));
            Assert.True(services.Contains(ServiceDescriptor.Singleton<IPipelineLoader, PipelineLoader>(), comparer));
            Assert.True(services.Contains(ServiceDescriptor.Singleton<IPipelineRunner, PipelineRunner>(), comparer));
        }

        [Fact]
        public void AddConfigHost_GeneratedContainerDisposesCorrectly()
        {
            // Arrange
            ServiceCollection services = new ServiceCollection();
            services.AddConfigHost();
            Container container = new Container(r => r.Populate(services)); // PipelineLoader requires an IContainer instance

            PipelineLoader pipelineLoader = container.GetInstance<IPipelineLoader>() as PipelineLoader;

            // Act
            container.Dispose();

            // Assert
            Assert.True(pipelineLoader.IsDisposed);
        }
    }
}
