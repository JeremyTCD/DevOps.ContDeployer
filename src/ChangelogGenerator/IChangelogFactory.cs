namespace JeremyTCD.ContDeployer.Plugin.ChangelogGenerator
{
    public interface IChangelogFactory
    {
        IChangelog Build(string pattern, string changelogText);
    }
}
