using JeremyTCD.DotNetCore.Utils;
using StructureMap;
using StructureMap.Pipeline;
using StructureMap.Query;
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
    }
}
