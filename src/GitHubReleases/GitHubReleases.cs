using JeremyTCD.ContDeployer.PluginTools;
using System;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer.Plugin.GitHubReleases
{
    public class GitHubReleases //: IPlugin
    {
        //private PipelineContext _pipelineContext { get; set; }
        //private GitHubReleaseChangelogAdapterOptions _options { get; set; }

        ///// <summary>
        ///// Tags head
        ///// </summary>
        ///// <param name="sharedData"></param>
        ///// <param name="steps"></param>
        //public void Run(PipelineContext pipelineContext, StepContext stepContext)
        //{
        //    _options = stepContext.Options as GitHubReleaseChangelogAdapterOptions;

        //    if (_options == null)
        //    {
        //        throw new InvalidOperationException($"{nameof(GitHubReleaseChangelogAdapterOptions)} required");
        //    }

        //    _pipelineContext = pipelineContext;

        //    pipelineContext.SharedData.TryGetValue(nameof(Changelog), out object changelogObject);
        //    Changelog changelog = changelogObject as Changelog;
        //    if (changelog == null)
        //    {
        //        throw new InvalidOperationException($"No {nameof(Changelog)} in {nameof(pipelineContext.SharedData)}");
        //    }

        //    List<ChangelogGenerator.Version> versions = changelog.Versions.ToList();

        //    List<Release> releases = GetGitHubReleases();

        //    // iterate through versions, ensure that each has a corresponding release with the same notes
        //}

        //public List<Release> GetGitHubReleases()
        //{
        //    GitHubClient client = new GitHubClient(new ProductHeaderValue(nameof(ContDeployer)));
        //    Credentials credentials = new Credentials(_options.Token);
        //    client.Credentials = credentials;

        //    List<Release> releases = client.Repository.Release.GetAll(_options.Owner, _options.Repository).Result.ToList();

        //    return releases;
        //}

    }
}
