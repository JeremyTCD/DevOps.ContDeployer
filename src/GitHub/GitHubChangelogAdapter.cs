using JeremyTCD.ContDeployer.Plugin.Changelog;
using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Logging;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JeremyTCD.ContDeployer.Plugin.GitHub
{
    public class GitHubChangelogAdapter : IPlugin
    {
        private GitHubChangelogAdapterOptions _options { get; }
        private IChangelog _changelog { get; }
        private IGitHubClient _gitHubClient { get; }        

        /// <summary>
        /// Creates a <see cref="GitHubChangelogAdapter"/> instance
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="IStepContext.PluginOptions"/> is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <see cref="IPipelineContext.SharedData"/> does not contain <see cref="IChangelog"/> instance
        /// </exception>
        public GitHubChangelogAdapter(IPipelineContext pipelineContext, IStepContext stepContext, 
            IGitHubClientFactory gitHubClientFactory) : 
            base(pipelineContext, stepContext)
        {
            _options = stepContext.PluginOptions as GitHubChangelogAdapterOptions;

            if (_options == null)
            {
                throw new InvalidOperationException($"{nameof(GitHubChangelogAdapterOptions)} required");
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
        /// Compares <see cref="_changelog"/> and gitHub releases. Adds <see cref="GitHubPlugin"/> step if a version
        /// has no corresponding release or if a version's notes are inconsistent with its release.
        /// </summary>
        public void Run(IPipelineContext pipelineContext, IStepContext stepContext)
        {
            List<IVersion> versions = _changelog.Versions.ToList();
  
            Dictionary<string, Release> releases = GetGitHubReleases();

            GitHubPluginOptions gitHubReleasesOptions = new GitHubPluginOptions
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
                    gitHubReleasesOptions.ModifiedReleases.Add(new ModifiedRelease()
                    {
                        ReleaseUpdate = new ReleaseUpdate
                        {
                            Body = version.Notes,
                            Name = release.Name,
                            Draft = release.Draft,
                            Prerelease = release.Prerelease,
                            TargetCommitish = release.TargetCommitish
                        },
                        Id = release.Id
                    });

                    StepContext.
                        Logger.
                        LogInformation($"Version \"{name}\" has been updated");
                }
            }

            if (gitHubReleasesOptions.NewReleases.Count > 0 || gitHubReleasesOptions.ModifiedReleases.Count > 0)
            {
                IStep gitHubReleasesStep = PipelineContext.
                    StepFactory.
                    Build(nameof(GitHubPlugin), gitHubReleasesOptions);
                PipelineContext.
                    Steps.
                    AddFirst(gitHubReleasesStep);

                StepContext.
                    Logger.
                    LogInformation($"Added {nameof(GitHubPlugin)} step");
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
                Result.
                ToDictionary(release => release.Name);
        }
    }
}
