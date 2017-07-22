using JeremyTCD.PipelinesCE.Core;
using System;

namespace JeremyTCD.PipelinesCE.Plugin.Git
{
    public class GitPluginOptions : IPluginOptions
    {
        /// <summary>
        /// Tag name
        /// </summary>
        public virtual string TagName { get; set; } 

        /// <summary>
        /// Tag signature name
        /// </summary>
        public virtual string Name { get; set; } 

        /// <summary>
        /// Tag signature email
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Commit-ish referring to commit that tag will point to
        /// </summary>
        public virtual string Commitish { get; set; } = "HEAD";

        public virtual void Validate()
        {
            if (string.IsNullOrEmpty(TagName))
            {
                throw new Exception($"{nameof(GitPluginOptions)}: {nameof(TagName)} cannot be null or empty");
            }

            if (string.IsNullOrEmpty(Name))
            {
                throw new Exception($"{nameof(GitPluginOptions)}: {nameof(Name)} cannot be null or empty");
            }

            if (string.IsNullOrEmpty(Email))
            {
                throw new Exception($"{nameof(GitPluginOptions)}: {nameof(Email)} cannot be null or empty");
            }

            if (string.IsNullOrEmpty(Commitish))
            {
                throw new Exception($"{nameof(GitPluginOptions)}: {nameof(Commitish)} cannot be null or empty");
            }
        }
    }
}
