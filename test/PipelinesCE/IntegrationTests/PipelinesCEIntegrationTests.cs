using StructureMap;
using System;
using System.IO;
using Xunit;

namespace JeremyTCD.PipelinesCE.Tests.IntegrationTests
{
    [Collection(nameof(PipelinesCECollection))]
    public class PipelinesCEIntegrationTests
    {
        private IContainer _container { get; }
        private string _projectFileName { get; } = "PipelinesCE.csproj";
        private string _tempDir { get; set; }
        private string _assemblyName { get; } = "JeremyTCD.Test.dll";
        private string _testProj { get; } = @"<Project Sdk=""Microsoft.NET.Sdk"">
            <PropertyGroup>
            <TargetFramework>netcoreapp1.1</TargetFramework>
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

        public PipelinesCEIntegrationTests(PipelinesCEFixture fixture)
        {
            fixture.ResetTempDir();
            _container = fixture.GetContainer();
            _tempDir = fixture.TempDir;
        }

        [Fact]
        public void Start_ThrowsExceptionIfPipelinesCEProjectFileDoesNotExist()
        {
            // Arrange
            PipelinesCE pipelinesCE = _container.GetInstance<PipelinesCE>();

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => pipelinesCE.Run(null));
        }

        [Fact]
        public void Start_ThrowsExceptionIfMoreThanOnePipelinesCEProjectFilesExist()
        {
            // Arrange
            PipelinesCE pipelinesCE = _container.GetInstance<PipelinesCE>();
            File.WriteAllText(_projectFileName, "");
            Directory.CreateDirectory("test");
            File.WriteAllText("test/PipelinesCE.csproj", "");

            // Act and Asssert
            Assert.Throws<InvalidOperationException>(() => pipelinesCE.Run(null));
        }

        [Fact]
        public void Start_BuildsPipelinesCEProject()
        {
            // Arrange
            File.WriteAllText(_projectFileName, _testProj);
            PipelinesCE pipelinesCE = _container.GetInstance<PipelinesCE>();

            // Act 
            // TODO test will fail while PluginOptions is null, must specify pipeline name
            pipelinesCE.Run(null);

            // Assert
            Assert.True(File.Exists($"{_tempDir}/bin/Release/netcoreapp1.1/{_assemblyName}"));
        }
    }
}
