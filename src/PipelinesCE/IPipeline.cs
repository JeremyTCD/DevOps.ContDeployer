using JeremyTCD.PipelinesCE.PluginTools;
using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE
{
    public interface IPipeline 
    {
        LinkedList<IStep> Steps { get; set; }
        string Name { get; set; }

        void Run();
    }
}
