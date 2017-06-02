using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.CommandLineUtils;
using Moq;
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

        public CommandsIntegrationTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void RootCommand_UnexpectedOptionThrowsExceptionAndPrintsHintToConsole()
        {
            // Arrange
            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            RootCommand rootCommand = new RootCommand(null, _cluService);

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

            RootCommand rootCommand = new RootCommand(null, _cluService);

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

            RootCommand rootCommand = new RootCommand(null, _cluService);

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

            RootCommand rootCommand = new RootCommand(null, _cluService);

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

            RootCommand rootCommand = new RootCommand(null, _cluService);

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
        public void RunCommand_CallsPipelinesCERunWithSpecifiedOptions(string[] arguments, bool dryRun, bool verbose, string pipeline, string project)
        {
            // Arrange
            Mock<PipelinesCE> mockPipelinesCE = _mockRepository.Create<PipelinesCE>(null, null, null, null, null, null, null, null);
            mockPipelinesCE.
                Setup(p => p.Run(It.Is<PipelineOptions>(o => o.DryRun == dryRun && o.Verbose == verbose && o.Pipeline == pipeline && o.Project == project)));

            RootCommand rootCommand = new RootCommand(mockPipelinesCE.Object, _cluService);

            // Act
            rootCommand.Execute(arguments);

            // Assert
            _mockRepository.VerifyAll();
        }

        public static IEnumerable<object[]> RunCommandData()
        {
            string testProject = "testProject";
            string testPipeline = "testPipeline";
            string defaultProject = (new PipelineOptions()).Project;

            yield return new object[] { new string[] { _runCommandName }, false, false, null, defaultProject};
            yield return new object[] {new string[] {_runCommandName,
                $"-{Strings.VerboseOptionShortName}", $"-{Strings.DryRunOptionShortName}",
                $"-{Strings.ProjectOptionShortName}", testProject,
                $"-{Strings.PipelineOptionShortName}", testPipeline },
                true, true, testPipeline, testProject};
            yield return new object[] {new string[] {_runCommandName,
                $"--{Strings.VerboseOptionLongName}", $"--{Strings.DryRunOptionLongName}",
                $"--{Strings.ProjectOptionLongName}", testProject,
                $"--{Strings.PipelineOptionLongName}", testPipeline },
                true, true, testPipeline, testProject};
        }

        [Theory]
        [MemberData(nameof(RunCommandHelpData))]
        public void RunCommand_HelpPrintsHelpTextToConsole(string[] arguments)
        {
            // Arrange
            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            RootCommand rootCommand = new RootCommand(null, _cluService);

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
