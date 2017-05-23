using JeremyTCD.PipelinesCE.PluginTools;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.IO;
using Xunit;

namespace JeremyTCD.PipelinesCE.Plugin.MSBuild.Tests.IntegrationTests
{
    /// <summary>
    /// MSBuild.Client api isn't stable yet (as of 4.0.0-rc-netstandard2.0). All functions that utilize the api
    /// should be put through thorough integration tests.
    /// </summary>
    [Collection("MSBuildCollection")]
    public class MSBuildClientIntegrationTests
    {
        private string _tempDir { get; set; }
        private string _testProj { get; } = @"<Project Sdk=""Microsoft.NET.Sdk"">
            <PropertyGroup>
            <TargetFramework>net452</TargetFramework>
            <AssemblyName>JeremyTCD.Test</AssemblyName>
            <PackageId>JeremyTCD.Test</PackageId>
            <GenerateAssemblyConfigurationAttribute>false</GenerateAssemblyConfigurationAttribute>
            <GenerateAssemblyCompanyAttribute>false</GenerateAssemblyCompanyAttribute>
            <GenerateAssemblyProductAttribute>false</GenerateAssemblyProductAttribute>
            <Authors>JeremyTCD</Authors>
            <Copyright>Copyright©JeremyTCD2017</Copyright>
            <PackageLicenseUrl>https://test.com</PackageLicenseUrl>
            <PackageProjectUrl>https://test.com</PackageProjectUrl>
            <RepositoryUrl>https://test.com</RepositoryUrl>
            <Description>Atestpackage</Description>
            <GeneratePackageOnBuild>False</GeneratePackageOnBuild>
            </PropertyGroup>
            </Project>";

        public MSBuildClientIntegrationTests(MSBuildFixture fixture)
        {
            fixture.ResetTempDir();
            _tempDir = fixture.TempDir;
        }

        [Fact]
        public void Build_BuildsProject()
        {
            // Arrange
            string projFile = $"{_tempDir}/test.csproj";

            File.WriteAllText(projFile, _testProj);

            Mock<ILogger<ProcessManager>> mockLogger = new Mock<ILogger<ProcessManager>>();
            Mock<IOptions<SharedOptions>> mockOptions = new Mock<IOptions<SharedOptions>>();
            mockOptions.Setup(o => o.Value).Returns(new SharedOptions { DryRun = false });
            IProcessManager processManager = new ProcessManager(mockLogger.Object, mockOptions.Object);

            MSBuildClient client = new MSBuildClient(processManager);

            // Act
            client.Build(projFile);

            // Assert
            Assert.True(File.Exists($"{_tempDir}/bin/Debug/net452/JeremyTCD.Test.dll"));
            Assert.True(File.Exists($"{_tempDir}/bin/Debug/net452/JeremyTCD.Test.pdb"));
        }

        [Fact]
        public void Build_ConsidersArguments()
        {
            // Arrange
            string projFile = $"{_tempDir}/test.csproj";

            File.WriteAllText(projFile, _testProj);

            Mock<ILogger<ProcessManager>> mockLogger = new Mock<ILogger<ProcessManager>>();
            Mock<IOptions<SharedOptions>> mockOptions = new Mock<IOptions<SharedOptions>>();
            mockOptions.Setup(o => o.Value).Returns(new SharedOptions { DryRun = false });
            IProcessManager processManager = new ProcessManager(mockLogger.Object, mockOptions.Object);

            MSBuildClient client = new MSBuildClient(processManager);

            // Act
            client.Build(projFile, "/property:Configuration=Release");

            // Assert
            Assert.True(File.Exists($"{_tempDir}/bin/Release/net452/JeremyTCD.Test.dll"));
        }
    }
}
