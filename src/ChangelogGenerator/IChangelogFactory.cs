namespace JeremyTCD.ContDeployer.Plugin.Changelog
{
    public interface IChangelogFactory
    {
        IChangelog Build(string pattern, string changelogText);
    }
}
