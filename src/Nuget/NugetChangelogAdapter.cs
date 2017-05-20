using JeremyTCD.ContDeployer.Plugin.Changelog;
using JeremyTCD.ContDeployer.PluginTools;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading;

namespace JeremyTCD.ContDeployer.Plugin.Nuget
{
    public class NugetChangelogAdapter : IPlugin
    {
        private NugetChangelogAdapterOptions _options { get; }
        private INugetClient _nugetClient { get; }
        private IChangelog _changelog { get; }

        public NugetChangelogAdapter(IPipelineContext pipelineContext, IStepContext stepContext,
            INugetClient nugetClient) : base(pipelineContext, stepContext)
        {
            _options = stepContext.PluginOptions as NugetChangelogAdapterOptions;

            if (_options == null)
            {
                throw new InvalidOperationException($"{nameof(NugetChangelogAdapterOptions)} required");
            }

            pipelineContext.SharedData.TryGetValue(nameof(Changelog), out object changelogObject);
            _changelog = changelogObject as IChangelog;
            if (_changelog == null)
            {
                throw new InvalidOperationException($"No {nameof(Changelog)} in {nameof(pipelineContext.SharedData)}");
            }

            _nugetClient = nugetClient;
        }

        public void Run(IPipelineContext pipelineContext, IStepContext stepContext)
        {
            List<IPackageSearchMetadata> packageSearchMetadata = _nugetClient.GetPackageVersions(_options.Source, _options.PackageName, 
                CancellationToken.None);

            // Compare to changelog
            // - if a changelog version has no corresponding package, add nuget step 
        }
    }
}
