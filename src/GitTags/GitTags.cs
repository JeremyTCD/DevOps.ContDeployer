using JeremyTCD.ContDeployer.PluginTools;
using LibGit2Sharp;
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
        /// If <see cref="IStepContext.Options"/> is null
        /// </exception>
        public GitTags(IPipelineContext pipelineContext, IStepContext stepContext) : base(pipelineContext, stepContext)
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
            // TODO Provide other tag operations 
            //  - delete
            //  - change object that it points to
            //  - specifiy message
            //  - change message
            // TODO Double check time for consistency with what LibGit2Sharp expects
            if (!PipelineContext.SharedOptions.DryRun)
            {
                Signature signature = new Signature(_options.Name, _options.Email, DateTimeOffset.Now);
                GitObject target = PipelineContext.Repository.Lookup(_options.Commitish);
                PipelineContext.Repository.Tags.Add(_options.TagName, target, signature, "");
            }

            StepContext.
                Logger.
                LogInformation($"Annotated tag with name \"{_options.TagName}\" created. Signed with name \"{_options.Name}\" and " +
                $"email \"{_options.Email}\"");
        }
    }
}
