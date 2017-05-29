using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using Moq;
using Xunit;

namespace JeremyTCD.PipelinesCE.CommandLineApp.Tests.IntegrationTests
{
    /// <summary>
    /// Tests to ensure that the command line application is configured correctly. 
    /// </summary>
    public class CommandLineAppIntegrationTests
    {
        private MockRepository _mockRepository { get; }
        private string _runCommandName { get; } = nameof(RunCommand).Replace("Command", "").ToLowerInvariant();

        public CommandLineAppIntegrationTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void Execute_RunCommandCallsPipelinesCERun()
        {
            // Arrange
            Mock<PipelinesCE> mockPipelinesCE = _mockRepository.Create<PipelinesCE>(null, null, null);
            mockPipelinesCE.
                Setup(p => p.Run(It.Is<PipelineOptions>(o => o.DryRun == false && o.Verbose == false && o.Pipeline == null && o.Project == null)));

            RootCommand defaultCommand = new RootCommand(mockPipelinesCE.Object, new CommandLineUtilsService());

            // Act
            defaultCommand.Execute(new string[] { _runCommandName });

            // Assert
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void Execute_RunCommandWithShortOptionsCallsPipelinesCERunWithSpecifiedOptions()
        {
            // Arrange
            Mock<PipelinesCE> mockPipelinesCE = _mockRepository.Create<PipelinesCE>(null, null, null);
            mockPipelinesCE.
                Setup(p => p.Run(It.Is<PipelineOptions>(o => o.DryRun == false && o.Verbose == false && o.Pipeline == null && o.Project == null)));

            RootCommand defaultCommand = new RootCommand(mockPipelinesCE.Object, new CommandLineUtilsService());

            // Act
            defaultCommand.Execute(new string[] { _runCommandName });

            // Assert
            _mockRepository.VerifyAll();
        }

    }
}
