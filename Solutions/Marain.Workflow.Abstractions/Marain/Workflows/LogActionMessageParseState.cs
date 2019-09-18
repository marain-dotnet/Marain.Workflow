// <copyright file="LogActionMessageParseState.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    /// <summary>
    ///     State class used by the <see cref="LogAction.BuildMessage" /> method to
    ///     track state when parsing the messages for tokens.
    /// </summary>
    internal class LogActionMessageParseState
    {
        /// <summary>
        ///     Gets or sets the start index of the current token being parsed.
        /// </summary>
        public int IndexOfSubStart { get; set; }

        /// <summary>
        ///     Gets or sets the start index of the variable name that has been found inside
        ///     the current substitution token.
        /// </summary>
        public int IndexOfVariableNameStart { get; set; }

        /// <summary>
        ///     Gets or sets the parsing phase for the current position.
        /// </summary>
        public LogActionMessageParsePhase Phase { get; set; }
    }
}