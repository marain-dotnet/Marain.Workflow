// <copyright file="LogAction.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// An <see cref="IWorkflowAction" /> implementation that writes the message specified in
    /// the <see cref="LogMessage" /> (with token substitution) to the current Logger. See
    /// <see cref="LogMessage" /> for an explanation of how the token substitution works.
    /// </summary>
    public class LogAction : IWorkflowAction
    {
        /// <summary>
        /// The content type that will be used when serializing/deserializing.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.workflows.logaction";

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<LogAction> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogAction"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public LogAction(ILogger<LogAction> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        public string ContentType => RegisteredContentType;

        /// <inheritdoc />
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the log message. To include values from the <see cref="WorkflowInstance.Context" />,
        /// enclose them in curly brackets like this: <c>"To get the nama value from context do this: {context.Name}."</c>.
        /// </summary>
        public string LogMessage { get; set; }

        /// <inheritdoc />
        public Task<WorkflowActionResult> ExecuteAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
        {
            this.logger.LogDebug(BuildMessage(this.LogMessage, instance));
            return Task.FromResult(WorkflowActionResult.Empty);
        }

        /// <summary>
        /// Constructs the message to write from the <see cref="LogMessage" /> and the
        /// current <see cref="WorkflowInstance" /> and <see cref="IWorkflowTrigger" />.
        /// </summary>
        /// <param name="logMessage">
        /// The message to format.
        /// </param>
        /// <param name="instance">
        /// The current <see cref="WorkflowInstance" /> that this action is executing on.
        /// </param>
        /// <returns>
        /// The full message, with any substitution tokens replaced with the appropriate values
        /// from the instance and/or trigger.
        /// </returns>
        private static string BuildMessage(string logMessage, WorkflowInstance instance)
        {
            var state = new LogActionMessageParseState();
            var sb = new StringBuilder();

            for (int i = 0; i < logMessage.Length; ++i)
            {
                char currentChar = logMessage[i];
                switch (state.Phase)
                {
                    case LogActionMessageParsePhase.LookingForSubstitutionStart:
                        if (currentChar == '{')
                        {
                            state.Phase = LogActionMessageParsePhase.LookingForContextStart;
                            state.IndexOfSubStart = i;
                        }
                        else
                        {
                            sb.Append(currentChar);
                        }

                        break;

                    case LogActionMessageParsePhase.LookingForContextStart:
                        if (logMessage.Substring(i, 8) == "context.")
                        {
                            i += 8;
                            state.IndexOfVariableNameStart = i;
                            state.Phase = LogActionMessageParsePhase.LookingForVariableNameEnd;
                        }
                        else
                        {
                            sb.Append(currentChar);
                            state.Phase = LogActionMessageParsePhase.LookingForSubstitutionStart;
                        }

                        break;

                    case LogActionMessageParsePhase.LookingForVariableNameEnd:
                        if (currentChar == '}')
                        {
                            string variableName = logMessage[state.IndexOfVariableNameStart..i];
                            if (instance.Context.TryGetValue(variableName, out _))
                            {
                                sb.Append(instance.Context[variableName]);
                            }
                            else
                            {
                                sb.Append(logMessage, state.IndexOfSubStart, i - state.IndexOfSubStart + 1);
                            }

                            state.Phase = LogActionMessageParsePhase.LookingForSubstitutionStart;
                        }

                        break;
                }
            }

            if (state.Phase != LogActionMessageParsePhase.LookingForSubstitutionStart)
            {
                sb.Append(logMessage, state.IndexOfSubStart, logMessage.Length - state.IndexOfSubStart);
            }

            return sb.ToString();
        }
    }
}