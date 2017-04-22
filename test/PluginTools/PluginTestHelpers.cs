using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.IO;

namespace JeremyTCD.ContDeployer.PluginTools.Tests
{
    /// <summary>
    /// Helper functions for testing plugins
    /// </summary>
    public class PluginTestHelpers
    {
        /// <summary>
        /// Creates a <see cref="PipelineContext"/> instance. Uses empty structures and defaults for dependencies and
        /// properties.
        /// </summary>
        /// <returns></returns>
        public static PipelineContext CreatePipelineContext(bool dryRun = false)
        {
            Mock<ILogger<ProcessManager>> mockLogger = new Mock<ILogger<ProcessManager>>();
            Mock<IOptions<SharedOptions>> mockSharedOptions = new Mock<IOptions<SharedOptions>>();
            mockSharedOptions.Setup(o => o.Value).Returns(new SharedOptions
            {
                DryRun = dryRun
            });
            ProcessManager processManager = new ProcessManager(mockLogger.Object, mockSharedOptions.Object);
            Repository repository = new Repository(Directory.GetCurrentDirectory());

            List<Step> steps = new List<Step>();
            Mock<IOptions<PipelineContextOptions>> mockPipelineContextOptions = new Mock<IOptions<PipelineContextOptions>>();
            mockPipelineContextOptions.Setup(p => p.Value).Returns(new PipelineContextOptions
            {
                Steps = steps
            });

            return new PipelineContext(null, processManager, repository, mockPipelineContextOptions.Object);
        }

        /// <summary>
        /// Creates a <see cref="StepContext"/> instance. 
        /// </summary>
        /// <param name="pluginOptions"></param>
        /// <returns></returns>
        public static StepContext CreateStepContext(IPluginOptions pluginOptions = null)
        {
            Mock<ILogger> mockLogger = new Mock<ILogger>();

            return new StepContext
            {
                Options = pluginOptions,
                Logger = mockLogger.Object
            };
        }
    }
}
