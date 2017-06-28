using JeremyTCD.PipelinesCE.Plugin.Changelog;
using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading;

namespace JeremyTCD.PipelinesCE.Plugin.Nuget
{
    public class NugetChangelogAdapter : IPlugin
    {
        private INugetClient _nugetClient { get; }
        private IChangelog _changelog { get; set; }

        public NugetChangelogAdapter(INugetClient nugetClient) 
        {
            _nugetClient = nugetClient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pipelineContext"></param>
        /// <param name="stepContext"></param>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="IStepContext.PluginOptions"/> is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <see cref="IPipelineContext.SharedData"/> does not contain <see cref="IChangelog"/> instance
        /// </exception>
        public void Run(IPipelineContext pipelineContext, IStepContext stepContext)
        {
            NugetChangelogAdapterOptions options = stepContext.PluginOptions as NugetChangelogAdapterOptions;

            if (options == null)
            {
                throw new InvalidOperationException($"{nameof(NugetChangelogAdapterOptions)} required");
            }

            pipelineContext.SharedData.TryGetValue(nameof(Changelog), out object changelogObject);
            _changelog = changelogObject as IChangelog;
            if (_changelog == null)
            {
                throw new InvalidOperationException($"No {nameof(Changelog)} in {nameof(pipelineContext.SharedData)}");
            }

            List<IPackageSearchMetadata> packageSearchMetadata = _nugetClient.GetPackageVersions(options.Source, options.PackageName, 
                CancellationToken.None);

            // Compare to changelog
            // - if a changelog version has no corresponding package, add nuget step 
        }
    }
}
