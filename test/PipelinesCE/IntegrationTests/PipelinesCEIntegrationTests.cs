using StructureMap;
using Xunit;

namespace JeremyTCD.PipelinesCE.Tests.IntegrationTests
{
    // TODO read up on how to test service configuration for structuremap?
    /// <summary>
    /// The goal of these tests is to ensure that <see cref="ServiceCollectionExtensions"/> configures PipelinesCE
    /// services properly.
    /// </summary>
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
        public void Run_BuildsPipelinesCEProject()
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
