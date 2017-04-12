using JeremyTCD.ContDeployer.PluginTools;
using System;
using System.Collections.Generic;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using LibGit2Sharp.Extensions;

namespace JeremyTCD.ContDeployer.Plugin.ChangelogDiffGenerator
{
    public class ChangelogDiffGenerator : PluginBase
    {
        public ChangelogDiffGeneratorOptions Options { get; set; }
        public ILogger<ChangelogDiffGenerator> Logger { get; set; }

        public ChangelogDiffGenerator(ChangelogDiffGeneratorOptions options, ILogger<ChangelogDiffGenerator> logger, IRepository repository) :
            base(repository)
        {
            Options = options;
            Logger = logger;
        }

        public override void Run(Dictionary<string, object> sharedData, LinkedList<PipelineStep> steps)
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
            ChangelogDiff diff = headChangelogMetadata.Diff(previousChangelogMetadata);

            if (diff.AddedVersions.Count > 1)
            {
                throw new Exception($"Cannot add more than 1 version at a time. Deploy manually.");
            }

            if (diff.RemovedVersions.Count > 0)
            {
                // Removal does not make sense since certain objects are permanent
                throw new Exception($"Cannot remove versions. Deploy manually.");
            }

            sharedData[nameof(ChangelogDiff)] = diff;
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
