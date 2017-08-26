using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JeremyTCD.PipelinesCE.Core
{
    public class PipelineRunner : IPipelineRunner
    {
        private IStepGraphFactory _stepGraphFactory { get; }
        public ILoggerFactory _loggerFactory { get; }

        public PipelineRunner(IStepGraphFactory stepGraphFactory, ILoggerFactory loggerFactory)
        {
            _stepGraphFactory = stepGraphFactory;
            _loggerFactory = loggerFactory;
        }

        // TODO rewrite
        /// <summary>
        /// Runs steps. If an exception is thrown, all steps that have not started are labelled as cancelled. Also, a shared <see cref="CancellationToken"/>
        /// has its <see cref="CancellationToken.IsCancellationRequested"/> set to true, allowing individual steps to 
        /// cancel as soon as possible.
        /// </summary>
        /// <param name="pipelineContext"></param>
        public void Run(Pipeline pipeline, IPipelineContext pipelineContext)
        {
            // Create StepGraph
            StepGraph stepGraph = _stepGraphFactory.CreateFromComposableGroup(pipeline);

            // Setup steps
            foreach (Step step in stepGraph)
            {
                step.Logger = _loggerFactory.CreateLogger(step.Name);
            }

            // Sort steps topologically
            stepGraph.SortTopologically();

            // Create CancellationTokenSource
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                // Run steps
                foreach (Step step in stepGraph)
                {
                    if (step.Dependencies.Count == 0)
                    {
                        step.Task = Task.Factory.StartNew(() => RunStep(step, pipelineContext, cts),
                            step.RunEvenIfFaultOccurs ? CancellationToken.None : cts.Token);
                    }
                    else
                    {

                        step.Task = Task.Factory.ContinueWhenAll(step.Dependencies.Select(p => p.Task).ToArray(),
                            (Task[] antecedents) => RunStep(step, pipelineContext, cts, antecedents),
                            step.RunEvenIfFaultOccurs ? CancellationToken.None : cts.Token);
                    }
                }

                Task.WaitAll(stepGraph.Select(s => s.Task).ToArray());
            }
        }

        private void RunStep(Step step, IPipelineContext pipelineContext, CancellationTokenSource cts, Task[] antecedents = null)
        {
            try
            {
                step.Logger.LogInformation(Strings.Log_RunningStep, step.Name);
                // Running steps can poll cts.Token.IsCancellationRequested to facilitate graceful terminations in the event of exceptions in other steps. 
                step.Run(pipelineContext, cts.Token);
                step.Logger.LogInformation(Strings.Log_FinishedRunningStep, step.Name);
            }
            catch
            {
                cts.Cancel();
                throw;
            }
        }
    }
}
