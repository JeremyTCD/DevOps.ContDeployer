using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.Newtonsoft.Json.Utils;
using JeremyTCD.PipelinesCE.Tools;
using JeremyTCD.DotNetCore.ProjectRunner;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using StructureMap;
using System;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.PipelinesCE.CommandLineApp.Tests.IntegrationTests
{
    /// <summary>
    /// Tests to ensure that <see cref="Program"/> commands have been configured correctly. Note that <see cref="CommandLineApplication"/>
    /// formats output depending on option names and descriptions. This means that output strings must
    /// be normalized before being compared with expected strings.
    /// </summary>
    public class CommandsIntegrationTests
    {
        private MockRepository _mockRepository { get; }
        private ICommandLineUtilsService _cluService { get; }
        private StringService _stringService { get; }

        public CommandsIntegrationTests()
        {
            _stringService = new StringService();
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
            _cluService = new CommandLineUtilsService(_mockRepository.Create<ILoggingService<CommandLineUtilsService>>().Object);
        }

        [Fact]
        public void RootCommand_UnexpectedOptionThrowsExceptionAndPrintsHintToConsole()
        {
            // Arrange
            ThreadSpecificStringWriter tssw = new ThreadSpecificStringWriter();
            Console.SetOut(tssw);
            Container container = new Container(new CommandLineAppRegistry());

            RootCommand rootCommand = container.GetInstance<RootCommand>();

            // Act and Assert
            Assert.Throws<CommandParsingException>(() => rootCommand.Execute(new string[] { "--test" }));
            tssw.Dispose();
            string output = tssw.ToString();
            string expected = $@"Specify --{Strings.OptionLongName_Help} for a list of available options and commands.";
            Assert.Equal(_stringService.RemoveWhiteSpace(expected), _stringService.RemoveWhiteSpace(output));
        }

        [Fact]
        public void RootCommand_LogsDebugMessageAndPrintsHelpTextToConsole()
        {
            // Arrange
            Mock<ILoggingService<RunCommand>> mockLoggingService = _mockRepository.Create<ILoggingService<RunCommand>>();
            mockLoggingService.Setup(l => l.IsEnabled(LogLevel.Debug)).Returns(true);
            mockLoggingService.Setup(l => l.LogDebug(Strings.Log_RunningCommand, Strings.CommandFullName_Root, $"{Strings.OptionLongName_Help}=\n" +
                $"{Strings.OptionLongName_Verbose}="));
            Container container = new Container(new CommandLineAppRegistry());
            container.Configure(registry => registry.For<ILoggingService<RunCommand>>().Use(mockLoggingService.Object).Singleton());

            ThreadSpecificStringWriter tssw = new ThreadSpecificStringWriter();
            Console.SetOut(tssw);

            RootCommand rootCommand = container.GetInstance<RootCommand>();

            // Act
            rootCommand.Execute(new string[0]);

            // Assert
            tssw.Dispose();
            string output = tssw.ToString();
            string expected = $@"{Strings.CommandFullName_Root} 1.0.0.0
                        Usage: {Strings.CommandName_Root} [options] [command]
                        Options:
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_Help, Strings.OptionLongName_Help)}     Show help information
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_Version, Strings.OptionLongName_Version)}  Show version information
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_Verbose, Strings.OptionLongName_Verbose)} {Strings.OptionDescription_Verbose} 
                        Commands:
                          { Strings.CommandName_Run}  {Strings.CommandDescription_Run}
                        Use ""{Strings.CommandName_Root} [command] --help"" for more information about a command.";
            Assert.Equal(_stringService.RemoveWhiteSpace(expected), _stringService.RemoveWhiteSpace(output));
        }

        [Theory]
        [MemberData(nameof(RootCommandVersionData))]
        public void RootCommand_VersionPrintsVersionToConsole(string[] arguments)
        {
            // Arrange
            ThreadSpecificStringWriter tssw = new ThreadSpecificStringWriter();
            Console.SetOut(tssw);
            Container container = new Container(new CommandLineAppRegistry());

            RootCommand rootCommand = container.GetInstance<RootCommand>();

            // Act
            rootCommand.Execute(arguments);

            // Assert
            // TODO test using regex after version format is decided on
            tssw.Dispose();
            string output = tssw.ToString();
            string expected = $@"{Strings.CommandFullName_Root}
                                1.0.0.0";
            Assert.Equal(_stringService.RemoveWhiteSpace(expected), _stringService.RemoveWhiteSpace(output));
        }

        public static IEnumerable<object[]> RootCommandVersionData()
        {
            yield return new object[] { new string[] { $"-{Strings.OptionShortName_Version}" } };
            yield return new object[] { new string[] { $"--{Strings.OptionLongName_Version}" } };
        }

        [Theory]
        [MemberData(nameof(RootCommandHelpData))]
        public void RootCommand_HelpPrintsHelpTextToConsole(string[] arguments)
        {
            // Arrange
            ThreadSpecificStringWriter tssw = new ThreadSpecificStringWriter();
            Console.SetOut(tssw);
            Container container = new Container(new CommandLineAppRegistry());

            RootCommand rootCommand = container.GetInstance<RootCommand>();

            // Act
            rootCommand.Execute(arguments);

            // Assert
            tssw.Dispose();
            string output = tssw.ToString();
            string expected = $@"{Strings.CommandFullName_Root} 1.0.0.0
                        Usage: {Strings.CommandName_Root} [options] [command]
                        Options:
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_Help, Strings.OptionLongName_Help)} Show help information
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_Version, Strings.OptionLongName_Version)} Show version information 
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_Verbose, Strings.OptionLongName_Verbose)} {Strings.OptionDescription_Verbose} 
                        Commands:
                          { Strings.CommandName_Run} {Strings.CommandDescription_Run}
                        Use ""{Strings.CommandName_Root} [command] --help"" for more information about a command.";
            Assert.Equal(_stringService.RemoveWhiteSpace(expected), _stringService.RemoveWhiteSpace(output));
        }

        public static IEnumerable<object[]> RootCommandHelpData()
        {
            yield return new object[] { new string[] { $"-{Strings.OptionShortName_Help}" } };
            yield return new object[] { new string[] { $"--{Strings.OptionLongName_Help}" } };
        }

        [Fact]
        public void RunCommand_UnexpectedOptionThrowsExceptionAndPrintsHintToConsole()
        {
            // Arrange
            ThreadSpecificStringWriter tssw = new ThreadSpecificStringWriter();
            Console.SetOut(tssw);
            Container container = new Container(new CommandLineAppRegistry());

            RootCommand rootCommand = container.GetInstance<RootCommand>();

            // Act and Assert
            Assert.Throws<CommandParsingException>(() => rootCommand.Execute(new string[] { "--test" }));
            tssw.Dispose();
            string output = tssw.ToString();
            string expected = $@"Specify --{Strings.OptionLongName_Help} for a list of available options and commands.";
            Assert.Equal(_stringService.RemoveWhiteSpace(expected), _stringService.RemoveWhiteSpace(output));
        }

        /// <summary>
        /// Ensures that CommandOptions are setup correctly and that arguments are processed correctly
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="pipelineOptions"></param>
        [Theory]
        [MemberData(nameof(RunCommandData))]
        public void RunCommand_LogsDebugMessageAndCallsRunnerRunWithSpecifiedOptions(string[] arguments, PipelineOptions pipelineOptions)
        {
            // Arrange
            string json = JsonConvert.SerializeObject(pipelineOptions, new PrivateFieldsJsonConverter());
            string[] stubArgs = new string[] { json };

            Mock<ILoggingService<RunCommand>> mockLoggingService = _mockRepository.Create<ILoggingService<RunCommand>>();
            mockLoggingService.Setup(l => l.IsEnabled(LogLevel.Debug)).Returns(false);

            Mock<IPathService> mockPathService = _mockRepository.Create<IPathService>();
            mockPathService.Setup(p => p.GetAbsolutePath(pipelineOptions.Project)).Returns(pipelineOptions.Project);

            Mock<Runner> mockRunner = _mockRepository.Create<Runner>(null, null, null, null, null, null, null);
            mockRunner.
                Setup(r => r.Run(pipelineOptions.Project, PipelineOptions.EntryAssemblyName, PipelineOptions.EntryClassName, It.IsAny<string>(), It.IsAny<string>(), stubArgs));

            Container container = new Container(new CommandLineAppRegistry());
            container.
                Configure(registry =>
                {
                    registry.For<Runner>().Use(mockRunner.Object).Singleton();
                    registry.For<ILoggingService<RunCommand>>().Use(mockLoggingService.Object).Singleton();
                    registry.For<IPathService>().Use(mockPathService.Object).Singleton();
                });

            RootCommand rootCommand = container.GetInstance<RootCommand>();

            // Act
            rootCommand.Execute(arguments);

            // Assert
            _mockRepository.VerifyAll();
        }

        public static IEnumerable<object[]> RunCommandData()
        {
            string testProject = "testProject";
            string testPipeline = "testPipeline";

            yield return new object[] { new string[] { Strings.CommandName_Run }, new PipelineOptions() };
            yield return new object[] {new string[] {Strings.CommandName_Run,
                $"-{Strings.OptionShortName_Verbose}",
                $"-{Strings.OptionShortName_DryRun}",
                $"-{Strings.OptionShortName_Project}", testProject,
                $"-{Strings.OptionShortName_Pipeline}", testPipeline },
                new PipelineOptions{
                    Verbose = true,
                    DryRun = true,
                    Project = testProject,
                    Pipeline = testPipeline
                }
            };
            yield return new object[] {new string[] {Strings.CommandName_Run,
                $"--{Strings.OptionLongName_Verbose}",
                $"--{Strings.OptionLongName_DryRun}",
                $"--{Strings.OptionLongName_Project}", testProject,
                $"--{Strings.OptionLongName_Pipeline}", testPipeline },
                new PipelineOptions{
                    Verbose = true,
                    DryRun = true,
                    Project = testProject,
                    Pipeline = testPipeline
                }
            };
            yield return new object[] { new string[] { Strings.CommandName_Run,
                $"-{Strings.OptionShortName_DryRunOff}",
                $"-{Strings.OptionShortName_VerboseOff}" },
                new PipelineOptions
                {
                    DryRun = false,
                    Verbose = false
                }
            };
            yield return new object[] { new string[] { Strings.CommandName_Run,
                $"--{Strings.OptionLongName_DryRunOff}",
                $"--{Strings.OptionLongName_VerboseOff}" },
                new PipelineOptions
                {
                    DryRun = false,
                    Verbose = false
                }
            };
        }

        [Theory]
        [MemberData(nameof(RunCommandHelpData))]
        public void RunCommand_HelpPrintsHelpTextToConsole(string[] arguments)
        {
            // Arrange
            ThreadSpecificStringWriter tssw = new ThreadSpecificStringWriter();
            Console.SetOut(tssw);
            Container container = new Container(new CommandLineAppRegistry());

            RootCommand rootCommand = container.GetInstance<RootCommand>();

            // Act
            rootCommand.Execute(arguments);

            // Assert
            tssw.Dispose();
            string output = tssw.ToString();
            string expected = $@"{Strings.CommandFullName_Run}
                        Usage: {Strings.CommandName_Root} {Strings.CommandName_Run} [options]
                        Options:
                          {_cluService.CreateOptionTemplate(Strings.OptionShortName_Help, Strings.OptionLongName_Help)}        Show help information
                          {_cluService.CreateOptionTemplate(Strings.OptionShortName_Project, Strings.OptionLongName_Project)}    {Strings.OptionDescription_Project}
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_Pipeline, Strings.OptionLongName_Pipeline)}   {Strings.OptionDescription_Pipeline}
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_DryRun, Strings.OptionLongName_DryRun)}      {Strings.OptionDescription_DryRun}
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_DryRunOff, Strings.OptionLongName_DryRunOff)}  {Strings.OptionDescription_DryRunOff}
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_Verbose, Strings.OptionLongName_Verbose)}    {Strings.OptionDescription_Verbose}
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_VerboseOff, Strings.OptionLongName_VerboseOff)}  {Strings.OptionDescription_VerboseOff}";
            Assert.Equal(_stringService.RemoveWhiteSpace(expected), _stringService.RemoveWhiteSpace(output));
        }

        public static IEnumerable<object[]> RunCommandHelpData()
        {
            yield return new object[] { new string[] { Strings.CommandName_Run, $"-{Strings.OptionShortName_Help}" } };
            yield return new object[] { new string[] { Strings.CommandName_Run, $"--{Strings.OptionLongName_Help}" } };
        }
    }
}
