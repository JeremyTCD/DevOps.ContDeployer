using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Logging;
using System;

namespace JeremyTCD.ContDeployer.Plugin.GitTags
{
    public class GitTags : IPlugin
    {
        /// <summary>
        /// Tags head
        /// </summary>
        /// <param name="sharedData"></param>
        /// <param name="steps"></param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <see cref="StepContext.Options"/> is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <see cref="GitTagsOptions.TagName"/> is null or empty
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown if git.exe fails 
        /// </exception>
        public void Run(PipelineContext pipelineContext, StepContext stepContext)
        {
            GitTagsOptions options = stepContext.Options as GitTagsOptions;

            if (options == null)
            {
                throw new InvalidOperationException($"{nameof(GitTagsOptions)} required");
            }

            string tagName = options.TagName;

            if (string.IsNullOrEmpty(tagName))
            {
                throw new InvalidOperationException($"{nameof(GitTagsOptions.TagName)} cannot be null or empty");
            }

            int exitCode = pipelineContext.
                            ProcessManager.
                            Execute("git.exe", $"tag {options.TagName}", 1000);

            if(exitCode == 0)
            {
                stepContext.
                    Logger.
                    LogInformation($"Lightweight tag with name \"{options.TagName}\" created");
            }
            else
            {
                throw new Exception($"Failed to create tag with name \"{options.TagName}\"");
            }
        }
    }
}
