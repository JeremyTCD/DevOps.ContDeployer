using NuGet.Common;
using NuGet.Protocol.Core.Types;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Xunit;

namespace JeremyTCD.PipelinesCE.Plugin.Nuget.Tests.IntegrationTests
{
    /// <summary>
    /// Nuget.Client api isn't stable yet (as of 4.0.0-rc-netstandard2.0). All functions that utilize the api
    /// should be put through thorough integration tests.
    /// </summary>
    [Collection("NugetCollection")]
    public class NugetClientIntegrationTests
    {
        private string _tempDir { get; set; }

        public NugetClientIntegrationTests(NugetFixture fixture)
        {
            fixture.ResetTempDir();
            _tempDir = fixture.TempDir;
        }

        [Fact]
        public void Pack_Packs()
        {
            // Arrange
            // Create test package 
            //string projFile = $"{_tempDir}/test.csproj";
            //string outputFile = $"{_tempDir}/test.nupkg";

            //File.WriteAllText(projFile, @"<Project Sdk=""Microsoft.NET.Sdk"">
            //<PropertyGroup>
            //<TargetFramework>net452</TargetFramework>
            //<AssemblyName>JeremyTCD.Test</AssemblyName>
            //<PackageId>JeremyTCD.Test</PackageId>
            //<GenerateAssemblyConfigurationAttribute>false</GenerateAssemblyConfigurationAttribute>
            //<GenerateAssemblyCompanyAttribute>false</GenerateAssemblyCompanyAttribute>
            //<GenerateAssemblyProductAttribute>false</GenerateAssemblyProductAttribute>
            //<Authors>JeremyTCD</Authors>
            //<Copyright>Copyright©JeremyTCD2017</Copyright>
            //<PackageLicenseUrl>https://test.com</PackageLicenseUrl>
            //<PackageProjectUrl>https://test.com</PackageProjectUrl>
            //<RepositoryUrl>https://test.com</RepositoryUrl>
            //<Description>Atestpackage</Description>
            //<GeneratePackageOnBuild>False</GeneratePackageOnBuild>
            //</PropertyGroup>
            //</Project>");

            //NugetClient client = new NugetClient(NullLogger.Instance);

            // Act
            //client.Pack(projFile, outputFile, null);

            // Assert
        }

        [Fact]
        public void GetPackageVersions_GetsPackageVersions()
        {
            // Create test package 
            //File.WriteAllText(_tempDir, @"<Project Sdk=""Microsoft.NET.Sdk"">
            //<PropertyGroup>
            //<TargetFramework>net452</TargetFramework>
            //<AssemblyName>JeremyTCD.Test</AssemblyName>
            //<PackageId>JeremyTCD.Test</PackageId>
            //<GenerateAssemblyConfigurationAttribute>false</GenerateAssemblyConfigurationAttribute>
            //<GenerateAssemblyCompanyAttribute>false</GenerateAssemblyCompanyAttribute>
            //<GenerateAssemblyProductAttribute>false</GenerateAssemblyProductAttribute>
            //<Authors>JeremyTCD</Authors>
            //<Copyright>Copyright©JeremyTCD2017</Copyright>
            //<PackageLicenseUrl>https://test.com</PackageLicenseUrl>
            //<PackageProjectUrl>https://test.com</PackageProjectUrl>
            //<RepositoryUrl>https://test.com</RepositoryUrl>
            //<Description>Atestpackage</Description>
            //<GeneratePackageOnBuild>False</GeneratePackageOnBuild>
            //</PropertyGroup>
            //</Project>");

            //NugetClient client = new NugetClient(NullLogger.Instance);

            //List<IPackageSearchMetadata> result = client.
            //    GetPackageVersions("https://api.nuget.org/v3/index.json", "Microsoft.AspNet.Razor", CancellationToken.None);
        }
    }
}
