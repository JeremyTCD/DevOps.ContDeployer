using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JeremyTCD.PipelinesCE.Core
{
    public class PipelineRunner : IPipelineRunner
    {
        private IStepGraphFactory _stepGraphFactory { get; }
        private CancellationTokenSource _cts { get; set; }
        public ILoggerFactory _loggerFactory { get; }

        public PipelineRunner(IStepGraphFactory stepGraphFactory, ILoggerFactory loggerFactory)
        {
            _stepGraphFactory = stepGraphFactory;
            _loggerFactory = loggerFactory;
            _cts = new CancellationTokenSource();
        }

        /// <summary>
        /// Runs steps. 
        /// 
        /// If an exception is thrown, all steps that have not started and that do not have <see cref="Step.RunEvenIfFaultOccurs"/> set to true
        /// are cancelled. Also, running steps can poll <see cref="CancellationToken.IsCancellationRequested"/> to exit gracefully.
        /// </summary>
        /// <param name="pipeline"></param>
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

            // Run steps
            try
            {
                foreach (Step step in stepGraph)
                {
                    if (step.Dependencies.Count == 0)
                    {
                        step.Task = Task.Factory.StartNew(() => RunStep(step, pipelineContext, _cts),
                            step.RunEvenIfFaultOccurs ? CancellationToken.None : _cts.Token);
                    }
                    else
                    {
                        step.Task = Task.Factory.ContinueWhenAll(step.Dependencies.Select(p => p.Task).ToArray(),
                            _ => RunStep(step, pipelineContext, _cts),
                            step.RunEvenIfFaultOccurs ? CancellationToken.None : _cts.Token);
                    }
                }

                Task.WaitAll(stepGraph.Select(s => s.Task).ToArray());
            }
            finally
            {
                _cts.Dispose();
            }
        }

        public void Cancel()
        {
            _cts.Cancel();
        }

        private void RunStep(Step step, IPipelineContext pipelineContext, CancellationTokenSource cts)
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
