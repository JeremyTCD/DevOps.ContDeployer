using JeremyTCD.PipelinesCE.Tools;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Octokit;
using System;

namespace JeremyTCD.PipelinesCE.Plugin.GitHub
{
    public class GitHubPlugin : IPlugin
    {
        private GitHubPluginOptions _options { get; set; }
        private IGitHubClient _gitHubClient { get; set; }
        private IStepContext _stepContext { get; set; }
        private IPipelineContext _pipelineContext { get; set; }
        private IGitHubClientFactory _gitHubClientFactory { get; }

        /// <summary>
        /// Creates a <see cref="GitHubPlugin"/> instance
        /// </summary>
        public GitHubPlugin(IGitHubClientFactory gitHubClientFactory) 
        {
            _gitHubClientFactory = gitHubClientFactory;
        }

        /// <summary>
        /// Compares <see cref="_changelog"/> and gitHub releases. Adds <see cref="GitHubPlugin"/> step if a version
        /// has no corresponding release or if a version's notes are inconsistent with its release.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="IStepContext.PluginOptions"/> is null
        /// </exception>
        public void Run(IPipelineContext pipelineContext, IStepContext stepContext)
        {
            _options = stepContext.PluginOptions as GitHubPluginOptions;

            if (_options == null)
            {
                throw new InvalidOperationException($"{nameof(GitHubChangelogAdapterOptions)} required");
            }

            _stepContext = stepContext;
            _pipelineContext = pipelineContext;
            _gitHubClient = _gitHubClientFactory.CreateClient(_options.Token);

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
                _stepContext.Logger.LogInformation("No new releases");
            }

            foreach (NewRelease newRelease in _options.NewReleases)
            {
                if (!_pipelineContext.PipelineOptions.DryRun)
                {
                    _gitHubClient.Repository.Release.Create(_options.Owner, _options.Repository, newRelease);
                }

                _stepContext.
                    Logger.
                    LogInformation($"Created release for repository \"{_options.Repository}\" with owner \"{_options.Owner}\":\n" +
                        $"{JsonConvert.SerializeObject(newRelease, Formatting.Indented)}");
            }
        }

        private void EditGitHubReleases()
        {
            if (_options.ModifiedReleases == null || _options.ModifiedReleases.Count == 0)
            {
                _stepContext.Logger.LogInformation("No release updates");
            }

            foreach (ModifiedRelease modifiedRelease in _options.ModifiedReleases)
            {
                if (!_pipelineContext.PipelineOptions.DryRun)
                {
                    _gitHubClient.Repository.Release.Edit(_options.Owner, _options.Repository, modifiedRelease.Id,
                        modifiedRelease.ReleaseUpdate);
                }

                _stepContext.
                    Logger.
                    LogInformation($"Modified release for repository \"{_options.Repository}\" with owner \"{_options.Owner}\":\n" +
                        $"{JsonConvert.SerializeObject(modifiedRelease, Formatting.Indented)}");
            }
        }
    }
}
