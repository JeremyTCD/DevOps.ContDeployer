using Octokit;

namespace JeremyTCD.PipelinesCE.Plugin.GitHub
{
    public class ModifiedRelease 
    {
        public virtual int Id { get; set; }
        public virtual ReleaseUpdate ReleaseUpdate { get; set; }
    }
}
