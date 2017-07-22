using JeremyTCD.PipelinesCE.Tools;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace JeremyTCD.PipelinesCE.Plugin.Git
{
    public class GitPlugin : IPlugin
    {
        private IRepository _repository { get; }

        public GitPlugin(IRepositoryFactory repositoryFactory)
        {
            _repository = repositoryFactory.Build(Directory.GetCurrentDirectory());
        }

        /// <summary>
        /// Tags head
        /// </summary>
        /// <param name="sharedData"></param>
        /// <param name="steps"></param>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="IStepContext.PluginOptions"/> is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <see cref="GitPluginOptions.TagName"/> is null or empty
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown if git.exe fails 
        /// </exception>
        public void Run(IPipelineContext pipelineContext, IStepContext stepContext)
        {
            GitPluginOptions options = stepContext.PluginOptions as GitPluginOptions;

            if (options == null)
            {
                throw new InvalidOperationException($"{nameof(GitPluginOptions)} required");
            }

            // TODO Provide other tag operations 
            //  - delete
            //  - change object that it points to
            //  - specifiy message
            //  - change message
            // TODO Double check time for consistency with what LibGit2Sharp expects
            if (!pipelineContext.PipelineOptions.DryRun)
            {
                Signature signature = new Signature(options.Name, options.Email, DateTimeOffset.Now);
                GitObject target = _repository.Lookup(options.Commitish);
                _repository.Tags.Add(options.TagName, target, signature, "");
            }

            stepContext.
                Logger.
                LogInformation($"Annotated tag with name \"{options.TagName}\" created. Signed with name \"{options.Name}\" and " +
                $"email \"{options.Email}\"");
        }
    }
}
