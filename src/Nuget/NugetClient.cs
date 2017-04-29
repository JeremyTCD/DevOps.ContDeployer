using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading;

namespace JeremyTCD.ContDeployer.Plugin.Nuget
{
    public class NugetClient : INugetClient
    {
        private ILogger _logger { get; }

        public NugetClient(ILogger logger)
        {
            _logger = logger;
        }

        public List<IPackageSearchMetadata> GetPackageVersions(string source, string packageName, CancellationToken cancellationToken)
        {
            SourceRepository sourceRepository = Repository.Factory.GetCoreV3(source);
            ListResource feed = sourceRepository.GetResourceAsync<ListResource>(cancellationToken).Result;

            if (feed == null)
            {
                throw new InvalidOperationException($"Nuget source \"{source}\" does not exist or exists but " +
                    $"does not support the list operation");
            }

            IEnumerableAsync<IPackageSearchMetadata> packageSearchMetadata = feed.
                ListAsync(packageName, true, true, true, _logger, cancellationToken).Result;
            IEnumeratorAsync<IPackageSearchMetadata> asyncEnumerator = packageSearchMetadata.GetEnumeratorAsync();

            List<IPackageSearchMetadata> result = new List<IPackageSearchMetadata>();

            while (asyncEnumerator.MoveNextAsync().Result)
            {
                result.Add(asyncEnumerator.Current);
            }

            return result;
        }
    }
}
