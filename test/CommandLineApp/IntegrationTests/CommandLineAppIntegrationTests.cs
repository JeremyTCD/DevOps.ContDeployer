using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        private ICommandLineUtilsService _cluService { get; }
        private static string _runCommandName { get; } = nameof(RunCommand).Replace("Command", "").ToLowerInvariant();
        private static string _runCommandFullName { get; } = $"{nameof(PipelinesCE)} {nameof(RunCommand).Replace("Command", "")}";
        private static string _rootCommandName { get; } = nameof(PipelinesCE).ToLowerInvariant();
        private static string _rootCommandFullName { get; } = nameof(PipelinesCE);
        private IServiceCollection _services { get; }

        public CommandsIntegrationTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

            _cluService = new CommandLineUtilsService(_mockRepository.Create<ILoggingService<CommandLineUtilsService>>().Object);
            Startup startup = new Startup();
            _services = new ServiceCollection();
            startup.ConfigureServices(_services);
        }

        [Fact]
        public void RootCommand_UnexpectedOptionThrowsExceptionAndPrintsHintToConsole()
        {
            // Arrange
            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            IServiceProvider serviceProvider = _services.BuildServiceProvider();

            RootCommand rootCommand = serviceProvider.GetService<RootCommand>();

            // Act and Assert
            Assert.Throws<CommandParsingException>(() => rootCommand.Execute(new string[] { "--test" }));
            string output = stringWriter.ToString();
            stringWriter.Dispose();
            string expected = $@"Specify --{Strings.HelpOptionLongName} for a list of available options and commands." + Environment.NewLine;
            Assert.Equal(expected, output);
        }

        [Fact]
        public void RootCommand_LogsDebugMessageAndPrintsHelpTextToConsole()
        {
            // Arrange
            Mock<ILoggingService<RunCommand>> mockLoggingService = _mockRepository.Create<ILoggingService<RunCommand>>();
            mockLoggingService.Setup(l => l.IsEnabled(LogLevel.Debug)).Returns(true);
            mockLoggingService.Setup(l => l.LogDebug(Strings.Log_RunningRootCommand, $"{Strings.HelpOptionLongName}=\n" +
                $"{Strings.VerboseOptionLongName}="));
            _services.AddSingleton(mockLoggingService.Object);
            IServiceProvider serviceProvider = _services.BuildServiceProvider();

            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            RootCommand rootCommand = serviceProvider.GetService<RootCommand>();

            // Act
            rootCommand.Execute(new string[0]);
            string output = stringWriter.ToString();
            stringWriter.Dispose();

            // Assert
            string expected = $@"{_rootCommandFullName} 1.0.0.0" + Environment.NewLine + Environment.NewLine +
                        $@"Usage: {_rootCommandName} [options] [command]" + Environment.NewLine + Environment.NewLine +
                        $@"Options:" + Environment.NewLine + 
                        $@"  { _cluService.CreateOptionTemplate(Strings.HelpOptionShortName, Strings.HelpOptionLongName)}     Show help information" + Environment.NewLine + 
                        $@"  { _cluService.CreateOptionTemplate(Strings.VersionOptionShortName, Strings.VersionOptionLongName)}  Show version information" + Environment.NewLine + Environment.NewLine +
                        $@"Commands:" + Environment.NewLine + 
                        $@"  { _runCommandName}  {Strings.RunCommandDescription}" + Environment.NewLine + Environment.NewLine +
                        $@"Use ""{_rootCommandName} [command] --help"" for more information about a command." + Environment.NewLine + Environment.NewLine;
            Assert.Equal(expected, output);
        }

        [Theory]
        [MemberData(nameof(RootCommandVersionData))]
        public void RootCommand_VersionPrintsVersionToConsole(string[] arguments)
        {
            // Arrange
            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            IServiceProvider serviceProvider = _services.BuildServiceProvider();

            RootCommand rootCommand = serviceProvider.GetService<RootCommand>();

            // Act
            rootCommand.Execute(arguments);
            string output = stringWriter.ToString();
            stringWriter.Dispose();

            // Assert
            // TODO test using regex after version format is decided on
            string expected = $@"{_rootCommandFullName}" + Environment.NewLine +
                        "1.0.0.0" + Environment.NewLine;
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
            IServiceProvider serviceProvider = _services.BuildServiceProvider();

            RootCommand rootCommand = serviceProvider.GetService<RootCommand>();

            // Act
            rootCommand.Execute(arguments);
            string output = stringWriter.ToString();
            stringWriter.Dispose();

            // Assert
            string expected = $@"{_rootCommandFullName} 1.0.0.0" + Environment.NewLine + Environment.NewLine +
                        $@"Usage: {_rootCommandName} [options] [command]" + Environment.NewLine + Environment.NewLine +
                        $@"Options:" + Environment.NewLine +
                        $@"  { _cluService.CreateOptionTemplate(Strings.HelpOptionShortName, Strings.HelpOptionLongName)}     Show help information" + Environment.NewLine +
                        $@"  { _cluService.CreateOptionTemplate(Strings.VersionOptionShortName, Strings.VersionOptionLongName)}  Show version information" + Environment.NewLine + Environment.NewLine +
                        $@"Commands:" + Environment.NewLine +
                        $@"  { _runCommandName}  {Strings.RunCommandDescription}" + Environment.NewLine + Environment.NewLine +
                        $@"Use ""{_rootCommandName} [command] --help"" for more information about a command." + Environment.NewLine + Environment.NewLine;
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
            IServiceProvider serviceProvider = _services.BuildServiceProvider();

            RootCommand rootCommand = serviceProvider.GetService<RootCommand>();

            // Act and Assert
            Assert.Throws<CommandParsingException>(() => rootCommand.Execute(new string[] { "--test" }));
            string output = stringWriter.ToString();
            stringWriter.Dispose();
            string expected = $@"Specify --{Strings.HelpOptionLongName} for a list of available options and commands." + Environment.NewLine;
            Assert.Equal(expected, output);
        }

        [Theory]
        [MemberData(nameof(RunCommandData))]
        public void RunCommand_LogsDebugMessageAndCallsPipelinesCERunWithSpecifiedOptions(string[] arguments, bool dryRun, bool verbose, string pipeline, 
            string project)
        {
            // Arrange
            Mock<ILoggingService<RunCommand>> mockLoggingService = _mockRepository.Create<ILoggingService<RunCommand>>();
            mockLoggingService.Setup(l => l.IsEnabled(LogLevel.Debug)).Returns(true);
            mockLoggingService.Setup(l => l.LogDebug(Strings.Log_RunningRunCommand, $"{Strings.HelpOptionLongName}=\n" +
                $"{Strings.ProjectOptionLongName}={project}\n" +
                $"{Strings.PipelineOptionLongName}={pipeline}\n" +
                $"{Strings.DryRunOptionLongName}={(dryRun ? "on" : "")}\n" +
                $"{Strings.VerboseOptionLongName}={(verbose ? "on" : "")}"));
            _services.AddSingleton(mockLoggingService.Object);
            IServiceProvider serviceProvider = _services.BuildServiceProvider();

            Mock<PipelinesCE> mockPipelinesCE = _mockRepository.Create<PipelinesCE>(null, null, null, null, null, null, null, null);
            mockPipelinesCE.
                Setup(p => p.Run(It.Is<PipelineOptions>(o => o.DryRun == dryRun && o.Pipeline == pipeline && o.Project == (project ?? (new PipelineOptions()).Project))));

            IContainer container = serviceProvider.GetService<IContainer>();
            container.Configure(registry => registry.For<PipelinesCE>().Use(mockPipelinesCE.Object));

            RootCommand rootCommand = serviceProvider.GetService<RootCommand>();

            // Act
            rootCommand.Execute(arguments);

            // Assert
            _mockRepository.VerifyAll();
            ILogger<CommandLineApp> logger = container.GetInstance<ILogger<CommandLineApp>>(); // Logger with arbitrary category
            CommandLineAppOptions claOptions = new CommandLineAppOptions();
            LogLevel logLevel = verbose ? claOptions.VerboseMinLogLevel : claOptions.DefaultMinLogLevel;
            Assert.True(logger.IsEnabled(logLevel) && (logLevel == LogLevel.Trace || !logger.IsEnabled(logLevel - 1)));
        }

        public static IEnumerable<object[]> RunCommandData()
        {
            string testProject = "testProject";
            string testPipeline = "testPipeline";
            CommandLineAppOptions claOptions = new CommandLineAppOptions();

            yield return new object[] { new string[] { _runCommandName }, false, false, null, null};
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
            IServiceProvider serviceProvider = _services.BuildServiceProvider();

            RootCommand rootCommand = serviceProvider.GetService<RootCommand>();

            // Act
            rootCommand.Execute(arguments);
            string output = stringWriter.ToString();
            stringWriter.Dispose();

            // Assert
            string expected = $@"{_runCommandFullName}" + Environment.NewLine + Environment.NewLine +
                        $@"Usage: {_rootCommandName} {_runCommandName} [options]" + Environment.NewLine + Environment.NewLine +
                        $@"Options:" + Environment.NewLine +
                        $@"  { _cluService.CreateOptionTemplate(Strings.HelpOptionShortName, Strings.HelpOptionLongName)}       Show help information" + Environment.NewLine +
                        $@"  {_cluService.CreateOptionTemplate(Strings.ProjectOptionShortName, Strings.ProjectOptionLongName)}   {Strings.ProjectOptionDescription}" + Environment.NewLine +
                        $@"  { _cluService.CreateOptionTemplate(Strings.PipelineOptionShortName, Strings.PipelineOptionLongName)}  {Strings.PipelineOptionDescription}" + Environment.NewLine +
                        $@"  { _cluService.CreateOptionTemplate(Strings.DryRunOptionShortName, Strings.DryRunOptionLongName)}     {Strings.DryRunDescription}" + Environment.NewLine +
                        $@"  { _cluService.CreateOptionTemplate(Strings.VerboseOptionShortName, Strings.VerboseOptionLongName)}   {Strings.VerboseOptionDescription}" + Environment.NewLine + Environment.NewLine;
            Assert.Equal(expected, output);
        }

        public static IEnumerable<object[]> RunCommandHelpData()
        {
            yield return new object[] { new string[] { _runCommandName, $"-{Strings.HelpOptionShortName}" } };
            yield return new object[] { new string[] { _runCommandName, $"--{Strings.HelpOptionLongName}" } };
        }
    }
}
