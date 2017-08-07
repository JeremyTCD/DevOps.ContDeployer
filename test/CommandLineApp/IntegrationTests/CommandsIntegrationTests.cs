using JeremyTCD.DotNetCore.ProjectHost;
using JeremyTCD.DotNetCore.Utils;
using JeremyTCD.Newtonsoft.Json.Utils;
using JeremyTCD.PipelinesCE.Core;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
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

        private ServiceProvider CreateServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddCommandLineApp();

            return services.BuildServiceProvider();
        }

        [Fact]
        public void RootCommand_UnexpectedOptionThrowsExceptionAndPrintsHintToConsole()
        {
            // Arrange
            ThreadSpecificStringWriter tssw = new ThreadSpecificStringWriter();
            Console.SetOut(tssw);
            ServiceProvider serviceProvider = CreateServiceProvider();

            RootCommand rootCommand = serviceProvider.GetService<RootCommand>();

            // Act and Assert
            Assert.Throws<CommandParsingException>(() => rootCommand.Execute(new string[] { "--test" }));
            serviceProvider.Dispose();
            tssw.Dispose();
            string output = tssw.ToString();
            string expected = $@"Specify --{Strings.OptionLongName_Help} for a list of available options and commands.";
            Assert.Equal(_stringService.RemoveWhiteSpace(expected), _stringService.RemoveWhiteSpace(output));
        }

        [Fact]
        public void RootCommand_PrintsHelpTextToConsole()
        {
            // Arrange
            ServiceProvider serviceProvider = CreateServiceProvider();

            ThreadSpecificStringWriter tssw = new ThreadSpecificStringWriter();
            Console.SetOut(tssw);

            RootCommand rootCommand = serviceProvider.GetService<RootCommand>();

            // Act
            rootCommand.Execute(new string[0]);

            // Assert
            serviceProvider.Dispose();
            tssw.Dispose();
            string output = tssw.ToString();
            string expected = $@"{Strings.CommandFullName_Root} 1.0.0.0
                        Usage: {Strings.CommandName_Root} [options] [command]
                        Options:
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_Help, Strings.OptionLongName_Help)}     Show help information
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_Version, Strings.OptionLongName_Version)}  Show version information
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
            ServiceProvider serviceProvider = CreateServiceProvider();

            RootCommand rootCommand = serviceProvider.GetService<RootCommand>();

            // Act
            rootCommand.Execute(arguments);

            // Assert
            serviceProvider.Dispose();
            tssw.Dispose();
            string output = tssw.ToString();
            // TODO test using regex after version format is decided on
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
            ServiceProvider serviceProvider = CreateServiceProvider();

            RootCommand rootCommand = serviceProvider.GetService<RootCommand>();

            // Act
            rootCommand.Execute(arguments);

            // Assert
            serviceProvider.Dispose();
            tssw.Dispose();
            string output = tssw.ToString();
            string expected = $@"{Strings.CommandFullName_Root} 1.0.0.0
                        Usage: {Strings.CommandName_Root} [options] [command]
                        Options:
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_Help, Strings.OptionLongName_Help)} Show help information
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_Version, Strings.OptionLongName_Version)} Show version information 
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
            ServiceProvider serviceProvider = CreateServiceProvider();

            RootCommand rootCommand = serviceProvider.GetService<RootCommand>();

            // Act and Assert
            Assert.Throws<CommandParsingException>(() => rootCommand.Execute(new string[] { "--test" }));
            serviceProvider.Dispose();
            tssw.Dispose();
            string output = tssw.ToString();
            string expected = $@"Specify --{Strings.OptionLongName_Help} for a list of available options and commands.";
            Assert.Equal(_stringService.RemoveWhiteSpace(expected), _stringService.RemoveWhiteSpace(output));
        }

        /// <summary>
        /// Ensures that CommandOptions are setup correctly and that arguments are processed correctly
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="pipelinesCEOptions"></param>
        [Theory]
        [MemberData(nameof(RunCommandData))]
        public void RunCommand_CallsRunnerRunWithSpecifiedOptions(string[] arguments, PipelinesCEOptions pipelinesCEOptions, SharedPluginOptions sharedPluginOptions)
        {
            // Arrange
            string pipelinesCEOptionsJson = JsonConvert.SerializeObject(pipelinesCEOptions, new PrivateFieldsJsonConverter());
            string sharedPluginOptionsJson = JsonConvert.SerializeObject(sharedPluginOptions, new PrivateFieldsJsonConverter());
            string[] stubArgs = new string[] { pipelinesCEOptionsJson, sharedPluginOptionsJson };

            Mock<Assembly> mockAssembly = _mockRepository.Create<Assembly>();

            Mock<IProjectLoader> mockProjectLoader = _mockRepository.Create<IProjectLoader>();
            mockProjectLoader.
                Setup(p => p.Load(pipelinesCEOptions.ProjectFile, "JeremyTCD.PipelinesCE.ConfigHost",
                    pipelinesCEOptions.Debug ? "Debug" : "Release")).
                Returns(mockAssembly.Object);

            Mock<IMethodRunner> mockMethodRunner = _mockRepository.Create<IMethodRunner>();
            mockMethodRunner.
                Setup(r => r.Run(mockAssembly.Object, "JeremyTCD.PipelinesCE.ConfigHost.ConfigHostStartup", It.IsAny<string>(), stubArgs));

            IServiceCollection services = new ServiceCollection();
            services.AddCommandLineApp();
            services.AddSingleton(mockProjectLoader.Object);
            services.AddSingleton(mockMethodRunner.Object);
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            RootCommand rootCommand = serviceProvider.GetService<RootCommand>();

            // Act
            rootCommand.Execute(arguments);

            // Assert
            _mockRepository.VerifyAll();
            serviceProvider.Dispose();
        }

        public static IEnumerable<object[]> RunCommandData()
        {
            string testProject = "testProject";
            string testPipeline = "testPipeline";
            string testLogFile = "testLogFile";
            string testArchiveFile = "testArchiveFile";

            yield return new object[] { new string[] { Strings.CommandName_Run }, new PipelinesCEOptions(), new SharedPluginOptions() };
            yield return new object[] {new string[] {Strings.CommandName_Run,
                $"-{Strings.OptionShortName_Verbose}",
                $"-{Strings.OptionShortName_DryRun}",
                $"-{Strings.OptionShortName_FileLogging}",
                $"-{Strings.OptionShortName_Debug}",
                $"-{Strings.OptionShortName_LogFile}", testLogFile,
                $"-{Strings.OptionShortName_ArchiveFile}", testArchiveFile,
                $"-{Strings.OptionShortName_ProjectFile}", testProject,
                $"-{Strings.OptionShortName_Pipeline}", testPipeline },
                new PipelinesCEOptions{
                    Verbose = true,
                    Debug = true,
                    FileLogging = true,
                    LogFile = testLogFile,
                    ArchiveFile = testArchiveFile,
                    ProjectFile = testProject,
                    Pipeline = testPipeline
                },
                new SharedPluginOptions
                {
                    DryRun = true
                }
            };
            yield return new object[] {new string[] {Strings.CommandName_Run,
                $"--{Strings.OptionLongName_Verbose}",
                $"--{Strings.OptionLongName_DryRun}",
                $"--{Strings.OptionLongName_FileLogging}",
                $"--{Strings.OptionLongName_Debug}",
                $"--{Strings.OptionLongName_LogFile}", testLogFile,
                $"--{Strings.OptionLongName_ArchiveFile}", testArchiveFile,
                $"--{Strings.OptionLongName_ProjectFile}", testProject,
                $"--{Strings.OptionLongName_Pipeline}", testPipeline },
                new PipelinesCEOptions{
                    Verbose = true,
                    Debug = true,
                    FileLogging = true,
                    LogFile = testLogFile,
                    ArchiveFile = testArchiveFile,
                    ProjectFile = testProject,
                    Pipeline = testPipeline
                },
                new SharedPluginOptions
                {
                    DryRun = true
                }
            };
            yield return new object[] { new string[] { Strings.CommandName_Run,
                $"-{Strings.OptionShortName_DryRunOff}",
                $"-{Strings.OptionShortName_VerboseOff}",
                $"-{Strings.OptionShortName_FileLoggingOff}",
                $"-{Strings.OptionShortName_DebugOff}" },
                new PipelinesCEOptions
                {
                    Verbose = false,
                    Debug = false,
                    FileLogging = false
                },
                new SharedPluginOptions
                {
                    DryRun = false
                }
            };
            yield return new object[] { new string[] { Strings.CommandName_Run,
                $"--{Strings.OptionLongName_DryRunOff}",
                $"--{Strings.OptionLongName_VerboseOff}" ,
                $"-{Strings.OptionShortName_FileLoggingOff}",
                $"--{Strings.OptionLongName_DebugOff}"},
                new PipelinesCEOptions
                {
                    Verbose = false,
                    Debug = false,
                    FileLogging = false
                },
                new SharedPluginOptions
                {
                    DryRun = false
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
            ServiceProvider serviceProvider = CreateServiceProvider();

            RootCommand rootCommand = serviceProvider.GetService<RootCommand>();

            // Act
            rootCommand.Execute(arguments);

            // Assert
            tssw.Dispose();
            string output = tssw.ToString();
            string expected = $@"{Strings.CommandFullName_Run}
                        Usage: {Strings.CommandName_Root} {Strings.CommandName_Run} [options]
                        Options:
                          {_cluService.CreateOptionTemplate(Strings.OptionShortName_Help, Strings.OptionLongName_Help)}        Show help information
                          {_cluService.CreateOptionTemplate(Strings.OptionShortName_ProjectFile, Strings.OptionLongName_ProjectFile)}    {Strings.OptionDescription_ProjectFile}
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_Pipeline, Strings.OptionLongName_Pipeline)}   {Strings.OptionDescription_Pipeline}
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_LogFile, Strings.OptionLongName_LogFile)}   {Strings.OptionDescription_LogFile}
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_ArchiveFile, Strings.OptionLongName_ArchiveFile)}   {Strings.OptionDescription_ArchiveFile}
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_DryRun, Strings.OptionLongName_DryRun)}      {Strings.OptionDescription_DryRun}
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_DryRunOff, Strings.OptionLongName_DryRunOff)}  {Strings.OptionDescription_DryRunOff}
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_Verbose, Strings.OptionLongName_Verbose)}    {Strings.OptionDescription_Verbose}
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_VerboseOff, Strings.OptionLongName_VerboseOff)}  {Strings.OptionDescription_VerboseOff}
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_Debug, Strings.OptionLongName_Debug)}    {Strings.OptionDescription_Debug}
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_DebugOff, Strings.OptionLongName_DebugOff)}  {Strings.OptionDescription_DebugOff}
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_FileLogging, Strings.OptionLongName_FileLogging)}    {Strings.OptionDescription_FileLogging}
                          { _cluService.CreateOptionTemplate(Strings.OptionShortName_FileLoggingOff, Strings.OptionLongName_FileLoggingOff)}  {Strings.OptionDescription_FileLoggingOff}";
            Assert.Equal(_stringService.RemoveWhiteSpace(expected), _stringService.RemoveWhiteSpace(output));
        }

        public static IEnumerable<object[]> RunCommandHelpData()
        {
            yield return new object[] { new string[] { Strings.CommandName_Run, $"-{Strings.OptionShortName_Help}" } };
            yield return new object[] { new string[] { Strings.CommandName_Run, $"--{Strings.OptionLongName_Help}" } };
        }
    }
}
