using Octokit;

namespace JeremyTCD.ContDeployer.Plugin.GitHub
{
    public class ModifiedRelease 
    {
        public virtual int Id { get; set; }
        public virtual ReleaseUpdate ReleaseUpdate { get; set; }
    }
}
