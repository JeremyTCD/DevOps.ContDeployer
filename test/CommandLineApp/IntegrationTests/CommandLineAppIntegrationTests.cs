using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StructureMap;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace JeremyTCD.PipelinesCE.CommandLineApp.Tests.IntegrationTests
{
    /// <summary>
    /// Tests to ensure that <see cref="CommandLineApp"/> commands have been configured correctly
    /// </summary>
    public class CommandsIntegrationTests
    {
        private MockRepository _mockRepository { get; }
        private ICommandLineUtilsService _cluService { get; } = new CommandLineUtilsService();
        private static string _runCommandName { get; } = nameof(RunCommand).Replace("Command", "").ToLowerInvariant();
        private static string _runCommandFullName { get; } = $"{nameof(PipelinesCE)} {nameof(RunCommand).Replace("Command", "")}";
        private static string _rootCommandName { get; } = nameof(PipelinesCE).ToLowerInvariant();
        private static string _rootCommandFullName { get; } = nameof(PipelinesCE);
        private IServiceProvider _serviceProvider { get; }

        public CommandsIntegrationTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

            Startup startup = new Startup();
            IServiceCollection services = new ServiceCollection();
            startup.ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void RootCommand_UnexpectedOptionThrowsExceptionAndPrintsHintToConsole()
        {
            // Arrange
            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            RootCommand rootCommand = _serviceProvider.GetService<RootCommand>();

            // Act and Assert
            Assert.Throws<CommandParsingException>(() => rootCommand.Execute(new string[] { "--test" }));
            string output = stringWriter.ToString();
            stringWriter.Dispose();
            string expected = $@"Specify --{Strings.HelpOptionLongName} for a list of available options and commands.
";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void RootCommand_PrintsHelpTextToConsole()
        {
            // Arrange
            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            RootCommand rootCommand = _serviceProvider.GetService<RootCommand>();

            // Act
            rootCommand.Execute(new string[0]);
            string output = stringWriter.ToString();
            stringWriter.Dispose();

            // Assert
            string expected =
$@"{_rootCommandFullName} 1.0.0.0

Usage: {_rootCommandName} [options] [command]

Options:
  {_cluService.CreateOptionTemplate(Strings.HelpOptionShortName, Strings.HelpOptionLongName)}     Show help information
  {_cluService.CreateOptionTemplate(Strings.VersionOptionShortName, Strings.VersionOptionLongName)}  Show version information

Commands:
  {_runCommandName}  {Strings.RunCommandDescription}

Use ""{_rootCommandName} [command] --help"" for more information about a command.

";
            Assert.Equal(expected, output);
        }

        [Theory]
        [MemberData(nameof(RootCommandVersionData))]
        public void RootCommand_VersionPrintsVersionToConsole(string[] arguments)
        {
            // Arrange
            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            RootCommand rootCommand = _serviceProvider.GetService<RootCommand>();

            // Act
            rootCommand.Execute(arguments);
            string output = stringWriter.ToString();
            stringWriter.Dispose();

            // Assert
            // TODO test using regex after version format is decided on
            string expected = $@"{_rootCommandFullName}
1.0.0.0
";
            Assert.Equal(expected, output);
        }

        public static IEnumerable<object[]> RootCommandVersionData()
        {
            yield return new object[] { new string[] { $"-{Strings.VersionOptionShortName}" } };
            yield return new object[] { new string[] { $"--{Strings.VersionOptionLongName}" } };
        }

        [Theory]
        [MemberData(nameof(RootCommandHelpData))]
        public void RootCommand_HelpPrintsHelpTextToConsole(string[] arguments)
        {
            // Arrange
            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            RootCommand rootCommand = _serviceProvider.GetService<RootCommand>();

            // Act
            rootCommand.Execute(arguments);
            string output = stringWriter.ToString();
            stringWriter.Dispose();

            // Assert
            string expected = 
$@"{_rootCommandFullName} 1.0.0.0

Usage: {_rootCommandName} [options] [command]

Options:
  {_cluService.CreateOptionTemplate(Strings.HelpOptionShortName, Strings.HelpOptionLongName)}     Show help information
  {_cluService.CreateOptionTemplate(Strings.VersionOptionShortName, Strings.VersionOptionLongName)}  Show version information

Commands:
  {_runCommandName}  {Strings.RunCommandDescription}

Use ""{_rootCommandName} [command] --help"" for more information about a command.

";
            Assert.Equal(expected, output);
        }

        public static IEnumerable<object[]> RootCommandHelpData()
        {
            yield return new object[] { new string[] { $"-{Strings.HelpOptionShortName}" } };
            yield return new object[] { new string[] { $"--{Strings.HelpOptionLongName}" } };
        }

        [Fact]
        public void RunCommand_UnexpectedOptionThrowsExceptionAndPrintsHintToConsole()
        {
            // Arrange
            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            RootCommand rootCommand = _serviceProvider.GetService<RootCommand>();

            // Act and Assert
            Assert.Throws<CommandParsingException>(() => rootCommand.Execute(new string[] { "--test" }));
            string output = stringWriter.ToString();
            stringWriter.Dispose();
            string expected = $@"Specify --{Strings.HelpOptionLongName} for a list of available options and commands.
";
            Assert.Equal(expected, output);
        }

        [Theory]
        [MemberData(nameof(RunCommandData))]
        public void RunCommand_CallsPipelinesCERunWithSpecifiedOptions(string[] arguments, bool dryRun, LogLevel logLevel, string pipeline, string project)
        {
            // Arrange
            Mock<PipelinesCE> mockPipelinesCE = _mockRepository.Create<PipelinesCE>(null, null, null, null, null, null, null, null);
            mockPipelinesCE.
                Setup(p => p.Run(It.Is<PipelineOptions>(o => o.DryRun == dryRun && o.Pipeline == pipeline && o.Project == project)));

            ICommandLineUtilsService cluService = _serviceProvider.GetService<ICommandLineUtilsService>();
            IOptions<CommandLineAppOptions> claOptionsAccessor = _serviceProvider.GetService<IOptions<CommandLineAppOptions>>();
            IContainer container = _serviceProvider.GetService<IContainer>();
            IContainer childContainer = container.CreateChildContainer();
            childContainer.Configure(registry => registry.For<PipelinesCE>().Use(mockPipelinesCE.Object));
            RunCommand runCommand = new RunCommand(cluService, claOptionsAccessor, childContainer);

            RootCommand rootCommand = new RootCommand(cluService, runCommand);

            // Act
            rootCommand.Execute(arguments);

            // Assert
            _mockRepository.VerifyAll();
            ILogger<CommandLineApp> logger = childContainer.GetInstance<ILogger<CommandLineApp>>();
            Assert.True(logger.IsEnabled(logLevel) && (logLevel == LogLevel.Trace || !logger.IsEnabled(logLevel - 1)));
        }

        public static IEnumerable<object[]> RunCommandData()
        {
            string testProject = "testProject";
            string testPipeline = "testPipeline";
            string defaultProject = (new PipelineOptions()).Project;
            CommandLineAppOptions claOptions = new CommandLineAppOptions();

            yield return new object[] { new string[] { _runCommandName }, false, claOptions.DefaultMinLogLevel, null, defaultProject};
            yield return new object[] {new string[] {_runCommandName,
                $"-{Strings.VerboseOptionShortName}", $"-{Strings.DryRunOptionShortName}",
                $"-{Strings.ProjectOptionShortName}", testProject,
                $"-{Strings.PipelineOptionShortName}", testPipeline },
                true, claOptions.VerboseMinLogLevel, testPipeline, testProject};
            yield return new object[] {new string[] {_runCommandName,
                $"--{Strings.VerboseOptionLongName}", $"--{Strings.DryRunOptionLongName}",
                $"--{Strings.ProjectOptionLongName}", testProject,
                $"--{Strings.PipelineOptionLongName}", testPipeline },
                true, claOptions.VerboseMinLogLevel, testPipeline, testProject};
        }

        [Theory]
        [MemberData(nameof(RunCommandHelpData))]
        public void RunCommand_HelpPrintsHelpTextToConsole(string[] arguments)
        {
            // Arrange
            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            RootCommand rootCommand = _serviceProvider.GetService<RootCommand>();

            // Act
            rootCommand.Execute(arguments);
            string output = stringWriter.ToString();
            stringWriter.Dispose();

            // Assert
            string expected =
$@"{_runCommandFullName}

Usage: {_rootCommandName} {_runCommandName} [options]

Options:
  {_cluService.CreateOptionTemplate(Strings.HelpOptionShortName, Strings.HelpOptionLongName)}       Show help information
  {_cluService.CreateOptionTemplate(Strings.ProjectOptionShortName, Strings.ProjectOptionLongName)}   {Strings.ProjectOptionDescription}
  {_cluService.CreateOptionTemplate(Strings.PipelineOptionShortName, Strings.PipelineOptionLongName)}  {Strings.PipelineOptionDescription}
  {_cluService.CreateOptionTemplate(Strings.DryRunOptionShortName, Strings.DryRunOptionLongName)}     {Strings.DryRunDescription}
  {_cluService.CreateOptionTemplate(Strings.VerboseOptionShortName, Strings.VerboseOptionLongName)}   {Strings.VerboseOptionDescription}

";
            Assert.Equal(expected, output);
        }

        public static IEnumerable<object[]> RunCommandHelpData()
        {
            yield return new object[] { new string[] { _runCommandName, $"-{Strings.HelpOptionShortName}" } };
            yield return new object[] { new string[] { _runCommandName, $"--{Strings.HelpOptionLongName}" } };
        }
    }
}
