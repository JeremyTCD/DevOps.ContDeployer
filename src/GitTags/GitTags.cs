using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Logging;
using System;

namespace JeremyTCD.ContDeployer.Plugin.GitTags
{
    public class GitTags : PluginBase
    {
        private GitTagsOptions _options { get; }

        /// <summary>
        /// Creates a <see cref="GitTags"/> instance
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="StepContext.Options"/> is null
        /// </exception>
        public GitTags(PipelineContext pipelineContext, StepContext stepContext) : base(pipelineContext, stepContext)
        {
            _options = stepContext.Options as GitTagsOptions;

            if (_options == null)
            {
                throw new InvalidOperationException($"{nameof(GitTagsOptions)} required");
            }
        }

        /// <summary>
        /// Tags head
        /// </summary>
        /// <param name="sharedData"></param>
        /// <param name="steps"></param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <see cref="GitTagsOptions.TagName"/> is null or empty
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown if git.exe fails 
        /// </exception>
        public override void Run()
        {
            string tagName = _options.TagName;

            if (string.IsNullOrEmpty(tagName))
            {
                throw new InvalidOperationException($"{nameof(GitTagsOptions.TagName)} cannot be null or empty");
            }

            int exitCode = PipelineContext.
                            ProcessManager.
                            Execute("git.exe", $"tag {_options.TagName}", 1000);

            if(exitCode == 0)
            {
                StepContext.
                    Logger.
                    LogInformation($"Lightweight tag with name \"{_options.TagName}\" created");
            }
            else
            {
                throw new Exception($"Failed to create tag with name \"{_options.TagName}\"");
            }
        }
    }
}
