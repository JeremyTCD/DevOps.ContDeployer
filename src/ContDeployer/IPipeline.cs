using JeremyTCD.ContDeployer.PluginTools;
using System.Collections.Generic;

namespace JeremyTCD.ContDeployer
{
    public interface IPipeline 
    {
        LinkedList<IStep> Steps { get; set; }
        string Name { get; set; }

        void Run();
    }
}
