namespace JeremyTCD.PipelinesCE.PluginTools
{
    public interface IProcessManager
    {
        int Execute(string fileName, string arguments, int timeoutMillis = int.MaxValue, bool executeOnDryRun = false);
    }
}
