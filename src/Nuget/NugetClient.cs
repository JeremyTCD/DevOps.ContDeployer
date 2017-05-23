using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Logging;
using NuGet.Commands;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace JeremyTCD.PipelinesCE.Plugin.Nuget
{
    /// <summary>
    /// Convenience methods for interacting with Nuget servers. Consider decomposing into more general functions
    /// and publishing as a separate project.
    /// </summary>
    public class NugetClient : INugetClient
    {
        private ILogger _logger { get; }

        public NugetClient(ILogger logger)
        {
            _logger = logger;
        }

        public List<IPackageSearchMetadata> GetPackageVersions(string source, string packageId, CancellationToken cancellationToken)
        {
            SourceRepository sourceRepository = Repository.Factory.GetCoreV2(new PackageSource(source));
            ListResource feed = sourceRepository.GetResourceAsync<ListResource>(cancellationToken).Result;

            if (feed == null)
            {
                throw new InvalidOperationException($"Nuget source \"{source}\" does not exist or exists but " +
                    $"does not support the list operation");
            }

            // Prefix with "PackageId" to only include exact package id matches in the result
            // https://docs.microsoft.com/en-us/nuget/consume-packages/finding-and-choosing-packages
            IEnumerableAsync<IPackageSearchMetadata> packageSearchMetadata = feed.
                ListAsync($"packageid:{packageId}", true, true, true, _logger, cancellationToken).Result;
            IEnumeratorAsync <IPackageSearchMetadata> asyncEnumerator = packageSearchMetadata.GetEnumeratorAsync();

            List<IPackageSearchMetadata> result = new List<IPackageSearchMetadata>();

            // TODO look through source, determine whether IEnumerableAsync is actually useful
            // why is it making multiple requests? multiple pages? each package 1 request?
            while (asyncEnumerator.MoveNextAsync().Result)
            {
                result.Add(asyncEnumerator.Current);
            }

            return result;
        }

        /// <summary>
        /// Packs a project into a nuget package
        /// </summary>
        /// <param name="path">
        /// Path of a project file (e.g csproj), .nuspec or project.json
        /// </param>
        /// <param name="propertyProvider">
        /// Function that returns property values for given keys
        /// </param>
        public void Pack(string path, string outputPath, Func<string, string> propertyProvider)
        {
            //BuildParameters parameters = new BuildParameters(new ProjectCollection())
            //{
                //Loggers = new ILogger[] { new ConsoleLogger() }
            //};
            //return BuildManager.DefaultBuildManager.Build(
            //    parameters,
            //    new BuildRequestData(path, properties, null, new[] { target }, null));
        }
    }
}
