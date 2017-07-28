namespace JeremyTCD.PipelinesCE.Core
{
    using NLog;
    using NLog.Targets;
    using System.Diagnostics;

    /// <summary>
    /// Writes log messages to the attached managed debugger.
    /// </summary>
    /// <example>
    /// <p>
    /// To set up the target in the <a href="config.html">configuration file</a>, 
    /// use the following syntax:
    /// </p>
    /// <code lang="XML" source="examples/targets/Configuration File/Debugger/NLog.config" />
    /// <p>
    /// This assumes just one target and a single rule. More configuration
    /// options are described <a href="config.html">here</a>.
    /// </p>
    /// <p>
    /// To set up the log target programmatically use code like this:
    /// </p>
    /// <code lang="C#" source="examples/targets/Configuration API/Debugger/Simple/Example.cs" />
    /// </example>
    [Target("Debugger")]
    public sealed class DebuggerTarget : TargetWithLayoutHeaderAndFooter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebuggerTarget" /> class.
        /// </summary>
        /// <remarks>
        /// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message}</code>
        /// </remarks>
        public DebuggerTarget() : base()
        {
            this.OptimizeBufferReuse = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DebuggerTarget" /> class.
        /// </summary>
        /// <remarks>
        /// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message}</code>
        /// </remarks>
        /// <param name="name">Name of the target.</param>
        public DebuggerTarget(string name) : this()
        {
            this.Name = name;
        }

        /// <summary>
        /// Initializes the target.
        /// </summary>
        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            if (this.Header != null)
            {
                Debugger.Log(LogLevel.Off.Ordinal, string.Empty, base.RenderLogEvent(this.Header, LogEventInfo.CreateNullEvent()) + "\n");
            }
        }

        /// <summary>
        /// Closes the target and releases any unmanaged resources.
        /// </summary>
        protected override void CloseTarget()
        {
            if (this.Footer != null)
            {
                Debugger.Log(LogLevel.Off.Ordinal, string.Empty, base.RenderLogEvent(this.Footer, LogEventInfo.CreateNullEvent()) + "\n");
            }

            base.CloseTarget();
        }

        /// <summary>
        /// Writes the specified logging event to the attached debugger.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            if (Debugger.IsLogging())
            {
                string logMessage = string.Empty;
                logMessage = base.RenderLogEvent(this.Layout, logEvent) + "\n";

                Debugger.Log(logEvent.Level.Ordinal, logEvent.LoggerName, logMessage);
            }
        }
    }
}
