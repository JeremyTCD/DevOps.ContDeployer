using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System.Linq;
using Xunit;

namespace JeremyTCD.PipelinesCE.Config.Tests.IntegrationTests
{
    public class ConfigServiceCollectionExtensionsIntegrationTests
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
            Assert.True(services.Contains(ServiceDescriptor.Singleton<IConfigRunner, ConfigRunner>(), comparer));
            Assert.True(services.Contains(ServiceDescriptor.Singleton<IConfigLoader, ConfigLoader>(), comparer));
        }

        [Fact]
        public void AddConfigHost_GeneratedContainerDisposesCorrectly()
        {
            // Arrange
            ServiceCollection services = new ServiceCollection();
            services.AddConfigHost();
            Container container = new Container(r => r.Populate(services)); // ConfigLoader requires an IContainer instance

            ConfigLoader pipelineLoader = container.GetInstance<IConfigLoader>() as ConfigLoader;

            // Act
            container.Dispose();

            // Assert
            Assert.True(pipelineLoader.IsDisposed);
        }
    }
}
