using JeremyTCD.ContDeployer.Plugin.ChangelogDiffGenerator;
using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JeremyTCD.ContDeployer.Plugin.TagGenerator
{
    public class TagGeneratorChangelogDiffAdapter : PluginBase
    {

        /// <summary>
        /// If <see cref="ChangelogDiff.AddedVersions"/> contains a <see cref="Version"/>,
        /// adds a <see cref="TagGenerator"/> pipeline step.
        /// </summary>
        /// <param name="sharedData"></param>
        /// <param name="steps"></param>
        public override void Run(PipelineContext pipelineContext, StepContext stepContext)
        {
            pipelineContext.SharedData.TryGetValue(nameof(ChangelogDiff), out object diffObject);
            ChangelogDiff diff = diffObject as ChangelogDiff;
            if(diff == null)
            {
                throw new InvalidOperationException($"No {nameof(ChangelogDiff)} in {nameof(pipelineContext.SharedData)}");
            }

            if (diff.AddedVersions.Count == 1)
            {
                TagGeneratorOptions tagGeneratorOptions = new TagGeneratorOptions
                {
                    TagName = diff.AddedVersions.First().SemVersion.ToString()
                };
                Step tagGeneratorStep = new Step(nameof(TagGenerator), tagGeneratorOptions);
                pipelineContext.Steps.AddFirst(tagGeneratorStep);

                stepContext.Logger.LogInformation($"Version added to changelog, added {nameof(TagGenerator)} step");
            }
            else if (diff.AddedVersions.Count > 1)
            {
                throw new InvalidOperationException($"{nameof(ChangelogDiff)} should not have more than 1 added versions");
            }
            else
            {
                stepContext.Logger.LogInformation($"No versions added to changelog");
            }
        }
    }
}
