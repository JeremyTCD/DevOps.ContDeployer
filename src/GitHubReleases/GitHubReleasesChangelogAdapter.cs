using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator;
using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Logging;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JeremyTCD.ContDeployer.Plugin.GitHubReleases
{
    public class GitHubReleasesChangelogAdapter : PluginBase
    {
        private GitHubReleasesChangelogAdapterOptions _options { get; }
        private IChangelog _changelog { get; }
        private IGitHubClient _gitHubClient { get; }        

        /// <summary>
        /// Creates a <see cref="GitHubReleasesChangelogAdapter"/> instance
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="StepContext.Options"/> is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <see cref="PipelineContext.SharedData"/> does not contain <see cref="IChangelog"/> instance
        /// </exception>
        public GitHubReleasesChangelogAdapter(PipelineContext pipelineContext, StepContext stepContext, 
            IGitHubClientFactory gitHubClientFactory) : 
            base(pipelineContext, stepContext)
        {
            _options = stepContext.Options as GitHubReleasesChangelogAdapterOptions;

            if (_options == null)
            {
                throw new InvalidOperationException($"{nameof(GitHubReleasesChangelogAdapterOptions)} required");
            }

            PipelineContext.SharedData.TryGetValue(nameof(Changelog), out object changelogObject);
            _changelog = changelogObject as IChangelog;
            if (_changelog == null)
            {
                throw new InvalidOperationException($"No {nameof(Changelog)} in {nameof(PipelineContext.SharedData)}");
            }

            _gitHubClient = gitHubClientFactory.CreateClient(_options.Token);
        }

        /// <summary>
        /// Compares <see cref="_changelog"/> and gitHub releases. Adds <see cref="GitHubReleases"/> step if a version
        /// has no corresponding release or if a version's notes are inconsistent with its release.
        /// </summary>
        public override void Run()
        {
            List<IVersion> versions = _changelog.Versions.ToList();
  
            Dictionary<string, Release> releases = GetGitHubReleases();

            GitHubReleasesOptions gitHubReleasesOptions = new GitHubReleasesOptions
            {
                Owner = _options.Owner,
                Repository = _options.Repository,
                Token = _options.Token
            };

            foreach (IVersion version in versions)
            {
                string name = version.SemVersion.ToString();

                releases.TryGetValue(name, out Release release);

                if (release == null)
                {
                    gitHubReleasesOptions.NewReleases.Add(new NewRelease(name)
                    {
                        Body = version.Notes,
                        Name = name,
                        Draft = false,
                        Prerelease = name.Contains('-'),
                        TargetCommitish = _options.Commitish // Ignored by gitHub api if tag already exists, otherwise creates a tag pointing to commitish
                    });

                    StepContext.
                        Logger.
                        LogInformation($"Version \"{name}\" has no corresponding gitHub release");
                }
                else if (release.Body != version.Notes)
                {
                    gitHubReleasesOptions.ReleaseUpdates.Add(new ReleaseUpdate()
                    {
                        Body = version.Notes,
                        Name = name,
                        Draft = false,
                        Prerelease = name.Contains('-'),
                        TargetCommitish = _options.Commitish
                    });

                    StepContext.
                        Logger.
                        LogInformation($"Version \"{name}\" has been updated");
                }
            }

            if (gitHubReleasesOptions.NewReleases.Count > 0 || gitHubReleasesOptions.ReleaseUpdates.Count > 0)
            {
                Step gitHubReleasesStep = new Step(nameof(GitHubReleases), gitHubReleasesOptions);
                PipelineContext.Steps.AddFirst(gitHubReleasesStep);

                StepContext.
                    Logger.
                    LogInformation($"Added {nameof(GitHubReleases)} step");
            }
            else
            {
                StepContext.
                    Logger.
                    LogInformation("GitHub releases consistent with changelog");
            }
        }

        /// <summary>
        /// Retrieves gitHub <see cref="Release"/>s for specified repository
        /// </summary>
        /// <returns>
        /// <see cref="Dictionary{String, Release}"/>
        /// </returns>
        private Dictionary<string, Release> GetGitHubReleases()
        {
            return _gitHubClient.
                Repository.
                Release.
                GetAll(_options.Owner, _options.Repository).
                Result.ToDictionary(release => release.TagName);
        }
    }
}
