using JeremyTCD.ContDeployer.Plugin.ChangelogGenerator;
using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JeremyTCD.ContDeployer.Plugin.TagGenerator
{
    public class TagGeneratorChangelogAdapter : IPlugin
    {
        /// <summary>
        /// Compares <see cref="Changelog"/> and git tags. If latest version has no corresponding tag (new version), 
        /// adds <see cref="TagGenerator"/> step to tag head.
        /// </summary>
        /// <param name="sharedData"></param>
        /// <param name="steps"></param>
        public void Run(PipelineContext pipelineContext, StepContext stepContext)
        {
            pipelineContext.SharedData.TryGetValue(nameof(Changelog), out object changelogObject);
            Changelog changelog = changelogObject as Changelog;
            if (changelog == null)
            {
                throw new InvalidOperationException($"No {nameof(Changelog)} in {nameof(pipelineContext.SharedData)}");
            }

            List<ChangelogGenerator.Version> versions = changelog.Versions.ToList();

            for (int i = 0; i < versions.Count; i++)
            {
                ChangelogGenerator.Version version = versions[i];

                if (pipelineContext.Repository.Tags[version.SemVersion.ToString()] == null)
                {
                    if (i == 0)
                    {
                        TagGeneratorOptions tagGeneratorOptions = new TagGeneratorOptions
                        {
                            TagName = version.SemVersion.ToString()
                        };
                        Step tagGeneratorStep = new Step(nameof(TagGenerator), tagGeneratorOptions);
                        pipelineContext.Steps.AddFirst(tagGeneratorStep);

                        stepContext.Logger.LogInformation($"New version \"{version.SemVersion.ToString()}\"" +
                            $"has no corresponding tag, added {nameof(TagGenerator)} step");
                    }
                    else
                    {
                        // TODO Each version should point to a commit, corresponding commits should be
                        // tagged
                        stepContext.Logger.LogWarning($"Version \"{version.SemVersion.ToString()}\" has no corresponding tag");
                    }
                }
            }
        }
    }
}
