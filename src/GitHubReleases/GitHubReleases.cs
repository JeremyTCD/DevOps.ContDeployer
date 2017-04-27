using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Octokit;
using System;

namespace JeremyTCD.ContDeployer.Plugin.GitHubReleases
{
    public class GitHubReleases : PluginBase
    {
        private GitHubReleasesOptions _options { get; }
        private IGitHubClient _gitHubClient { get; }

        /// <summary>
        /// Creates a <see cref="GitHubReleases"/> instance
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="IStepContext.Options"/> is null
        /// </exception>
        public GitHubReleases(IPipelineContext pipelineContext, IStepContext stepContext,
            IGitHubClientFactory gitHubClientFactory) :
            base(pipelineContext, stepContext)
        {
            _options = stepContext.Options as GitHubReleasesOptions;

            if (_options == null)
            {
                throw new InvalidOperationException($"{nameof(GitHubReleasesChangelogAdapterOptions)} required");
            }

            _gitHubClient = gitHubClientFactory.CreateClient(_options.Token);
        }

        /// <summary>
        /// Compares <see cref="_changelog"/> and gitHub releases. Adds <see cref="GitHubReleases"/> step if a version
        /// has no corresponding release or if a version's notes are inconsistent with its release.
        /// </summary>
        public override void Run()
        {
            // TODO provide other operations
            // - delete
            // TODO dry run
            CreateGitHubReleases();
            EditGitHubReleases();
        }

        private void CreateGitHubReleases()
        {
            if (_options.NewReleases == null || _options.NewReleases.Count == 0)
            {
                StepContext.Logger.LogInformation("No new releases");
            }

            foreach (NewRelease newRelease in _options.NewReleases)
            {
                _gitHubClient.Repository.Release.Create(_options.Owner, _options.Repository, newRelease);
                StepContext.
                    Logger.
                    LogInformation($"Created release for repository \"{_options.Repository}\" with owner \"{_options.Owner}\":\n" +
                        $"{JsonConvert.SerializeObject(newRelease, Formatting.Indented)}");
            }
        }

        private void EditGitHubReleases()
        {
            if (_options.ModifiedReleases == null || _options.ModifiedReleases.Count == 0)
            {
                StepContext.Logger.LogInformation("No release updates");
            }

            foreach (ModifiedRelease modifiedRelease in _options.ModifiedReleases)
            {
                _gitHubClient.Repository.Release.Edit(_options.Owner, _options.Repository, modifiedRelease.Id,
                    modifiedRelease.ReleaseUpdate);
                StepContext.
                    Logger.
                    LogInformation($"Modified release for repository \"{_options.Repository}\" with owner \"{_options.Owner}\":\n" +
                        $"{JsonConvert.SerializeObject(modifiedRelease, Formatting.Indented)}");
            }
        }
    }
}
