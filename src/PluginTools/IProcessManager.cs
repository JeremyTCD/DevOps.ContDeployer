namespace JeremyTCD.PipelinesCE.PluginTools
{
    public interface IProcessManager
    {
        int Run(string fileName, string arguments, int timeoutMillis = int.MaxValue);
    }
}
