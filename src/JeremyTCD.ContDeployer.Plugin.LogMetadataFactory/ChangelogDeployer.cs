using JeremyTCD.ContDeployer.PluginTools;
using System;
using System.Collections.Generic;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using LibGit2Sharp.Extensions;
using System.Text.RegularExpressions;
using Semver;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogDeployer
{
    public class ChangelogDeployer : PluginBase
    {
        public ChangelogDeployerOptions Options { get; set; }
        public ILogger<ChangelogDeployer> Logger { get; set; }

        public ChangelogDeployer(ChangelogDeployerOptions options, ILogger<ChangelogDeployer> logger, IRepository repository) :
            base(repository)
        {
            Options = options;
            Logger = logger;
        }

        public override void Run(LinkedList<PipelineStep> steps)
        {
            Logger.LogInformation("=== Running LogMetadataFactory ===");

            // TODO what if you're working another branch
            // Get changelog text
            string headChangelogText = GetHeadChangelogText();
            if (headChangelogText == null)
            {
                return;
            }
            string previousChangelogText = GetPreviousChangelogText();

            // Check if changelog has changed
            if (headChangelogText == previousChangelogText)
            {
                Logger.LogInformation($"No changes to changelog: {Options.FileName}");
                return;
            }

            // Build changelog metadata
            ChangelogMetadataFactory changelogMetadataFactory = new ChangelogMetadataFactory();
            ChangelogMetadata headChangelogMetadata = changelogMetadataFactory.Build(Options.Pattern, headChangelogText);
            ChangelogMetadata previousChangelogMetadata = previousChangelogText != null ?
                changelogMetadataFactory.Build(Options.Pattern, previousChangelogText) : null;

            // Diff changelog metadata
            ChangelogMetadataDiff diff = headChangelogMetadata.Diff(previousChangelogMetadata);

            // if new version added, add taggenerator, githubreleasepublisher and wtv publisher
            // if tag incremented, add taggenerator, githubreleasepublisher and wtv publisher
            // if changes made, add githubreleasepublisher


            //Console.WriteLine(newTree.Id);
        }

        private string GetHeadChangelogText()
        {
            Commit head = Repository.Lookup<Commit>("HEAD");
            if (head == null)
            {
                throw new Exception($"Repository has no commits");
            }
            GitObject headChangelogGitObject = head[Options.FileName]?.Target;
            if (headChangelogGitObject == null)
            {
                throw new Exception($"No file with name: {Options.FileName}");
            }

            return ((Blob)headChangelogGitObject).ReadAsString();
        }

        private string GetPreviousChangelogText()
        {
            Commit previous = Repository.Lookup<Commit>("HEAD^");
            if (previous == null)
            {
                Logger.LogInformation($"First commit");
                return null;
            }
            GitObject previousChangelogGitObject = previous[Options.FileName]?.Target;
            if (previous == null)
            {
                Logger.LogInformation($"First commit for: {Options.FileName}");
                return null;
            }

            return ((Blob)previousChangelogGitObject).ReadAsString();
        }
    }
}
