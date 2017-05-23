namespace JeremyTCD.PipelinesCE.Plugin.Changelog
{
    public interface IChangelogFactory
    {
        IChangelog CreateChangelog(string pattern, string changelogText);
    }
}
