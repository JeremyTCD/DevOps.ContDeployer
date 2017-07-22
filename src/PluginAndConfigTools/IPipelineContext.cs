using System.Collections.Generic;

namespace JeremyTCD.PipelinesCE.Tools
{
    public interface IPipelineContext
    {
        PipelineOptions PipelineOptions { get; set; }
        IDictionary<string, object> SharedData { get; set; }
    }
}
