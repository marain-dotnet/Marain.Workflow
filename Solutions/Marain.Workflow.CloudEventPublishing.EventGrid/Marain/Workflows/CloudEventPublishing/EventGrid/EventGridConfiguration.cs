// <copyright file="EventGridConfiguration.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

// Nullable is disabled here because this is populated by binding to configuration.
#nullable disable
namespace Marain.Workflows.CloudEventPublishing.EventGrid
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Event hub-related configuration data for workflow event publishing.
    /// </summary>
    public class EventGridConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether publishing to event grid is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the Uri of the endpoint for publishing to event grid.
        /// </summary>
        public Uri TopicEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the security key for the topic endpoint.
        /// </summary>
        /// <remarks>
        /// If this is set, the <see cref="KeyVaultName"/> and <see cref="TopicEndpointKeySecretName"/> will be ignored.
        /// If blank, those values are expected to be present.
        /// </remarks>
        public string TopicEndpointKey { get; set; }

        /// <summary>
        /// Gets or sets the name of the KeyVault that contains the key for accessing the topic endpoint.
        /// </summary>
        public string KeyVaultName { get; set; }

        /// <summary>
        /// Gets or sets the name of the secret containing the key for accessing the topic endpoint.
        /// </summary>
        public string TopicEndpointKeySecretName { get; set; }

        /// <summary>
        /// Validates the configuration.
        /// </summary>
        /// <exception cref="InvalidOperationException">The configuration is invalid.</exception>
        public void EnsureValid()
        {
            var errors = new List<string>();

            // If not enabled, don't bother with the remaining validation.
            if (this.Enabled)
            {
                if (this.TopicEndpoint == default)
                {
                    errors.Add("\t'TopicEndpoint' must be specified when using Event Grid configuration");
                }

                if (string.IsNullOrEmpty(this.TopicEndpointKey) && string.IsNullOrEmpty(this.KeyVaultName))
                {
                    errors.Add("\tBoth 'TopicEndpointKey' and 'KeyVaultName' are missing. You must specify one of these to use Event Grid publishing.");
                }

                if (string.IsNullOrEmpty(this.TopicEndpointKey) && !string.IsNullOrEmpty(this.KeyVaultName) && string.IsNullOrEmpty(this.TopicEndpointKeySecretName))
                {
                    errors.Add("\t'KeyVaultName' is specified, but 'TopicEndpointKeySecretName' is missing. When using KeyVault to store the topic endpoint key, you must also specifiy the corresponding Key Vault secret name.");
                }

                if (errors.Count > 0)
                {
                    errors.Insert(0, "One or more errors were found in the supplied Event Grid configuration:\n");
                    throw new InvalidOperationException(string.Join(Environment.NewLine, errors.ToArray()));
                }
            }
        }
    }
}
