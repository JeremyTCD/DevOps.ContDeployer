using JeremyTCD.ContDeployer.PluginTools;
using Microsoft.Extensions.Logging;
using System;

namespace JeremyTCD.ContDeployer.Plugin.TagGenerator
{
    public class TagGenerator : IPlugin
    {
        /// <summary>
        /// Tags head
        /// </summary>
        /// <param name="sharedData"></param>
        /// <param name="steps"></param>
        public void Run(PipelineContext pipelineContext, StepContext stepContext)
        {
            TagGeneratorOptions options = stepContext.Options as TagGeneratorOptions;

            if (options == null)
            {
                throw new InvalidOperationException($"{nameof(TagGeneratorOptions)} required");
            }

            string tagName = options.TagName;

            if (string.IsNullOrEmpty(tagName))
            {
                throw new InvalidOperationException($"{nameof(TagGeneratorOptions.TagName)} cannot be null or empty");
            }

            int exitCode = pipelineContext.
                            ProcessManager.
                            Execute("git.exe", $"tag {options.TagName}");

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
