using JeremyTCD.PipelinesCE.Plugin.Changelog;
using JeremyTCD.PipelinesCE.PluginAndConfigTools;
using Microsoft.Extensions.Logging;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JeremyTCD.PipelinesCE.Plugin.GitHub
{
    public class GitHubChangelogAdapter : IPlugin
    {
        private GitHubChangelogAdapterOptions _options { get; set;  }
        private IChangelog _changelog { get; set;  }
        private IGitHubClient _gitHubClient { get; set; }
        private IGitHubClientFactory _gitHubClientFactory { get; }        

        /// <summary>
        /// Creates a <see cref="GitHubChangelogAdapter"/> instance
        /// </summary>
        public GitHubChangelogAdapter(IGitHubClientFactory gitHubClientFactory) 
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
        /// <exception cref="InvalidOperationException">
        /// Thrown if <see cref="IPipelineContext.SharedData"/> does not contain <see cref="IChangelog"/> instance
        /// </exception>
        public void Run(IPipelineContext pipelineContext, IStepContext stepContext)
        {
            _options = stepContext.PluginOptions as GitHubChangelogAdapterOptions;
            if (_options == null)
            {
                throw new InvalidOperationException($"{nameof(GitHubChangelogAdapterOptions)} required");
            }

            pipelineContext.SharedData.TryGetValue(nameof(Changelog), out object changelogObject);
            _changelog = changelogObject as IChangelog;
            if (_changelog == null)
            {
                throw new InvalidOperationException($"No {nameof(Changelog)} in {nameof(PipelineContext.SharedData)}");
            }
            List<IVersion> versions = _changelog.Versions.ToList();
  
            _gitHubClient = _gitHubClientFactory.CreateClient(_options.Token);

            Dictionary<string, Release> releases = GetGitHubReleases();

            GitHubPluginOptions gitHubPluginOptions = new GitHubPluginOptions
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
                    gitHubPluginOptions.NewReleases.Add(new NewRelease(name)
                    {
                        Body = version.Notes,
                        Name = name,
                        Draft = false,
                        Prerelease = name.Contains("-"),
                        TargetCommitish = _options.Commitish // Ignored by gitHub api if tag already exists, otherwise creates a tag pointing to commitish
                    });

                    stepContext.
                        Logger.
                        LogInformation($"Version \"{name}\" has no corresponding gitHub release");
                }
                else if (release.Body != version.Notes)
                {
                    gitHubPluginOptions.ModifiedReleases.Add(new ModifiedRelease()
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

                    stepContext.
                        Logger.
                        LogInformation($"Version \"{name}\" has been updated");
                }
            }

            if (gitHubPluginOptions.NewReleases.Count > 0 || gitHubPluginOptions.ModifiedReleases.Count > 0)
            {
                IStep gitHubStep = new Step<GitHubPlugin>(gitHubPluginOptions);
                stepContext.
                    RemainingSteps.
                    AddFirst(gitHubStep);

                stepContext.
                    Logger.
                    LogInformation($"Added {nameof(GitHubPlugin)} step");
            }
            else
            {
                stepContext.
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
