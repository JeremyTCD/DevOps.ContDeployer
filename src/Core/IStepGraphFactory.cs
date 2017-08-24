namespace JeremyTCD.PipelinesCE.Core
{
    public interface IStepGraphFactory
    {
        StepGraph CreateFromComposableGroup(ComposableGroup composableGroup);
    }
}
