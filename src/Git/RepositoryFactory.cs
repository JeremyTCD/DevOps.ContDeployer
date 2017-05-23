using LibGit2Sharp;

namespace JeremyTCD.PipelinesCE.Plugin.Git
{
    public class RepositoryFactory : IRepositoryFactory
    {
        public IRepository Build(string path)
        {
            return new Repository(path);
        }
    }
}
