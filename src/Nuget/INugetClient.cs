using NuGet.Protocol.Core.Types;
using System.Collections.Generic;
using System.Threading;

namespace JeremyTCD.PipelinesCE.Plugin.Nuget
{
    public interface INugetClient
    {
        List<IPackageSearchMetadata> GetPackageVersions(string source, string packageName, CancellationToken cancellationToken);
    }
}
