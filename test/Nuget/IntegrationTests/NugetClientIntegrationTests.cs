using NuGet.Common;
using NuGet.Protocol.Core.Types;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace JeremyTCD.ContDeployer.Plugin.Nuget.Tests.IntegrationTests
{
    /// <summary>
    /// Nuget.Client api isn't stable yet (as of 4.0.0-rc-netstandard2.0). All functions that utilize the api
    /// should be put through thorough integration tests.
    /// </summary>
    public class NugetClientIntegrationTests
    {
        [Fact]
        public void GetPackageVersions_GetsPackageVersions()
        {
            // Create test package on staging.nuget.org

            NugetClient client = new NugetClient(NullLogger.Instance);

            List<IPackageSearchMetadata> result = client.
                GetPackageVersions("https://api.nuget.org/v3/index.json", "Microsoft.AspNet.Razor", CancellationToken.None);
        }
    }
}
