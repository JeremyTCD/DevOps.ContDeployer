using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator;
using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Logging;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JeremyTCD.ContDeployer.Plugin.GithubReleases
{
    public class GithubReleasesChangelogAdapter : PluginBase
    {
        private GithubReleasesChangelogAdapterOptions _options { get; }
        private Changelog _changelog { get; }
        private GitHubClient _githubClient { get; }
        
        /// <summary>
        /// Creates a <see cref="GithubReleasesChangelogAdapter"/> instance
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="StepContext.Options"/> is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <see cref="PipelineContext.SharedData"/> does not contain <see cref="Changelog"/> instance
        /// </exception>
        public GithubReleasesChangelogAdapter(PipelineContext pipelineContext, StepContext stepContext, GitHubClient githubClient) : 
            base(pipelineContext, stepContext)
        {
            _options = stepContext.Options as GithubReleasesChangelogAdapterOptions;

            if (_options == null)
            {
                throw new InvalidOperationException($"{nameof(GithubReleasesChangelogAdapterOptions)} required");
            }

            PipelineContext.SharedData.TryGetValue(nameof(Changelog), out object changelogObject);
            _changelog = changelogObject as Changelog;
            if (_changelog == null)
            {
                throw new InvalidOperationException($"No {nameof(Changelog)} in {nameof(PipelineContext.SharedData)}");
            }

            _githubClient = githubClient;
        }

        /// <summary>
        /// Compares <see cref="_changelog"/> and github releases. Adds <see cref="GithubReleases"/> step if a version
        /// has no corresponding release or if a version's notes are inconsistent with its release.
        /// </summary>
        public override void Run()
        {
            List<ChangelogGenerator.Version> versions = _changelog.Versions.ToList();

            Dictionary<string, Release> releases = GetGithubReleases();

            GithubReleasesOptions githubReleasesOptions = new GithubReleasesOptions
            {
                Owner = _options.Owner,
                Repository = _options.Repository,
                Token = _options.Token
            };

            foreach (ChangelogGenerator.Version version in versions)
            {
                string name = version.SemVersion.ToString();

                releases.TryGetValue(name, out Release release);

                if (release == null)
                {
                    githubReleasesOptions.NewReleases.Add(new NewRelease(name)
                    {
                        Body = version.Notes,
                        Name = name,
                        Draft = false,
                        Prerelease = name.Contains('-'),
                        TargetCommitish = _options.Commitish // Ignored by github api if tag already exists, otherwise creates a tag pointing to commitish
                    });

                    StepContext.
                        Logger.
                        LogInformation($"Version \"{name}\" has no corresponding github release");
                }
                else if (release.Body != version.Notes)
                {
                    githubReleasesOptions.ReleaseUpdates.Add(new ReleaseUpdate()
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

            if (githubReleasesOptions.NewReleases.Count > 0 || githubReleasesOptions.ReleaseUpdates.Count > 0)
            {
                Step githubReleasesStep = new Step(nameof(GithubReleases), githubReleasesOptions);
                PipelineContext.Steps.AddFirst(githubReleasesStep);

                StepContext.
                    Logger.
                    LogInformation($"Added {nameof(GithubReleases)} step");
            }
            else
            {
                StepContext.
                    Logger.
                    LogInformation("Github releases consistent with changelog");
            }
        }

        /// <summary>
        /// Retrieves github <see cref="Release"/>s for specified repository
        /// </summary>
        /// <returns>
        /// <see cref="Dictionary{String, Release}"/>
        /// </returns>
        private Dictionary<string, Release> GetGithubReleases()
        {
            Credentials credentials = new Credentials(_options.Token);
            _githubClient.Credentials = credentials;

            // TODO dry run mode
            return _githubClient.
                Repository.
                Release.
                GetAll(_options.Owner, _options.Repository).
                Result.ToDictionary(release => release.TagName);
        }
    }
}
