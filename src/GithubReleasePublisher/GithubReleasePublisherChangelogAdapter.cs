using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator;
using JeremyTCD.ContDeployer.PluginTools;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace JeremyTCD.ContDeployer.Plugin.GithubReleasePublisher
{
    public class GithubReleasePublisherChangelogAdapter : IPlugin
    {
        private PipelineContext _pipelineContext { get; set; }
        private GithubReleasePublisherChangelogAdapterOptions _options { get; set; }

        /// <summary>
        /// Tags head
        /// </summary>
        /// <param name="sharedData"></param>
        /// <param name="steps"></param>
        public void Run(PipelineContext pipelineContext, StepContext stepContext)
        {
            _options = stepContext.Options as GithubReleasePublisherChangelogAdapterOptions;

            if (_options == null)
            {
                throw new InvalidOperationException($"{nameof(GithubReleasePublisherChangelogAdapterOptions)} required");
            }

            _pipelineContext = pipelineContext;

            pipelineContext.SharedData.TryGetValue(nameof(Changelog), out object changelogObject);
            Changelog changelog = changelogObject as Changelog;
            if (changelog == null)
            {
                throw new InvalidOperationException($"No {nameof(Changelog)} in {nameof(pipelineContext.SharedData)}");
            }

            List<ChangelogGenerator.Version> versions = changelog.Versions.ToList();

            List<Release> releases = GetGithubReleases();

            // iterate through versions, ensure that each has a corresponding release with the same notes
        }

        public List<Release> GetGithubReleases()
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue(nameof(ContDeployer)));
            Credentials credentials = new Credentials(_options.Token);
            client.Credentials = credentials;

            List<Release> releases = client.Repository.Release.GetAll(_options.Owner, _options.Repository).Result.ToList();

            return releases;
        }
    }
}
