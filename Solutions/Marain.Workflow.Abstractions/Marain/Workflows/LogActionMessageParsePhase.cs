// <copyright file="LogActionMessageParsePhase.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    /// <summary>
    ///     An enumeration used by the <see cref="LogAction.BuildMessage" /> method to
    ///     track state when parsing the messages for tokens.
    /// </summary>
    internal enum LogActionMessageParsePhase
    {
        /// <summary>
        ///     This state indicates that we are searching for the start of a substitution token.
        /// </summary>
        LookingForSubstitutionStart,

        /// <summary>
        ///     This state indicates that we have found a substitution token and we are looking for
        ///     the word <c>context</c> within it.
        /// </summary>
        LookingForContextStart,

        /// <summary>
        ///     This state indicates that we have found a substitution token containing the <c>context</c>
        ///     keyword and we are now parsing the variable name until we find the end token.
        /// </summary>
        LookingForVariableNameEnd,
    }
}