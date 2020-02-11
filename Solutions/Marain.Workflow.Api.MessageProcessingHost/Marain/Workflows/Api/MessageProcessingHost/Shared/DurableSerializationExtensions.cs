// <copyright file="DurableSerializationExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.MessageProcessingHost.Shared
{
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Newtonsoft.Json;

    /// <summary>
    /// Extension methods for the various durable function client/context classes.
    /// </summary>
    public static class DurableSerializationExtensions
    {
        /// <summary>
        /// Starts a new instance of the specified orchestration, serializing the given input
        /// object using the supplied serialization settings.
        /// </summary>
        /// <typeparam name="T">The type of the input parameter.</typeparam>
        /// <param name="client">The <see cref="IDurableOrchestrationClient"/>.</param>
        /// <param name="orchestratorFunctionName">The name of the orchestration to start.</param>
        /// <param name="instanceId">The instance Id for the orchestration.</param>
        /// <param name="input">The input data.</param>
        /// <param name="jsonSerializerSettings">The serialization settings to use.</param>
        /// <returns>The instance Id of the newly started orchestration.</returns>
        /// <remarks>
        /// There is a feature PR that adds the ability to specify serialization settings to the
        /// durable functions host at startup. However, it only works for passing data from a starter
        /// to an orchestration, not from an orchestration to an activity.
        /// </remarks>
        public static Task<string> StartNewWithCustomSerializationSettingsAsync<T>(
            this IDurableOrchestrationClient client,
            string orchestratorFunctionName,
            string instanceId,
            T input,
            JsonSerializerSettings jsonSerializerSettings)
        {
            string serializedInput = JsonConvert.SerializeObject(input, jsonSerializerSettings);
            return client.StartNewAsync(orchestratorFunctionName, instanceId, serializedInput);
        }

        /// <summary>
        /// Gets input data from the supplied context and deserializes it using the specified
        /// settings.
        /// </summary>
        /// <typeparam name="T">The type of the input parameter.</typeparam>
        /// <param name="context">The <see cref="IDurableOrchestrationContext"/>.</param>
        /// <param name="jsonSerializerSettings">The serialization settings to use.</param>
        /// <returns>The deserialized input data.</returns>
        public static T GetInputWithCustomSerializationSettings<T>(
            this IDurableOrchestrationContext context,
            JsonSerializerSettings jsonSerializerSettings)
        {
            string serializedInput = context.GetInput<string>();
            return JsonConvert.DeserializeObject<T>(serializedInput, jsonSerializerSettings);
        }

        /// <summary>
        /// Gets input data from the supplied context and deserializes it using the specified
        /// settings.
        /// </summary>
        /// <typeparam name="T">The type of the input parameter.</typeparam>
        /// <param name="context">The <see cref="IDurableActivityContext"/>.</param>
        /// <param name="jsonSerializerSettings">The serialization settings to use.</param>
        /// <returns>The deserialized input data.</returns>
        public static T GetInputWithCustomSerializationSettings<T>(
            this IDurableActivityContext context,
            JsonSerializerSettings jsonSerializerSettings)
        {
            string serializedInput = context.GetInput<string>();
            return JsonConvert.DeserializeObject<T>(serializedInput, jsonSerializerSettings);
        }

        /// <summary>
        /// Starts a new instance of the specified activity, serializing the given input
        /// object using the supplied serialization settings.
        /// </summary>
        /// <typeparam name="TInput">The type of the input parameter.</typeparam>
        /// <param name="context">The orchestration context.</param>
        /// <param name="activityName">The name of the activity to start.</param>
        /// <param name="input">The input data.</param>
        /// <param name="jsonSerializerSettings">The serialization settings to use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task CallActivityWithCustomSerializationSettingsAsync<TInput>(
            this IDurableOrchestrationContext context,
            string activityName,
            TInput input,
            JsonSerializerSettings jsonSerializerSettings)
        {
            string serializedInput = JsonConvert.SerializeObject(input, jsonSerializerSettings);
            return context.CallActivityAsync(activityName, serializedInput);
        }

        /// <summary>
        /// Starts a new instance of the specified activity, serializing the given input
        /// object using the supplied serialization settings.
        /// </summary>
        /// <typeparam name="TInput">The type of the input parameter.</typeparam>
        /// <typeparam name="TOutput">The type of the return value.</typeparam>
        /// <param name="context">The orchestration context.</param>
        /// <param name="activityName">The name of the activity to start.</param>
        /// <param name="input">The input data.</param>
        /// <param name="jsonSerializerSettings">The serialization settings to use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task<TOutput> CallActivityWithCustomSerializationSettingsAsync<TInput, TOutput>(
            this IDurableOrchestrationContext context,
            string activityName,
            TInput input,
            JsonSerializerSettings jsonSerializerSettings)
        {
            string serializedInput = JsonConvert.SerializeObject(input, jsonSerializerSettings);
            return context.CallActivityAsync<TOutput>(activityName, serializedInput);
        }

        /// <summary>
        /// Starts a new instance of the specified sub-orchestration, serializing the given input
        /// object using the supplied serialization settings.
        /// </summary>
        /// <typeparam name="TInput">The type of the input parameter.</typeparam>
        /// <param name="context">The orchestration context.</param>
        /// <param name="subOrchestratorName">The name of the sub-orchestration to start.</param>
        /// <param name="input">The input data.</param>
        /// <param name="jsonSerializerSettings">The serialization settings to use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task CallSubOrchestratorWithCustomSerializationSettingsAsync<TInput>(
            this IDurableOrchestrationContext context,
            string subOrchestratorName,
            TInput input,
            JsonSerializerSettings jsonSerializerSettings)
        {
            string serializedInput = JsonConvert.SerializeObject(input, jsonSerializerSettings);
            return context.CallSubOrchestratorAsync(subOrchestratorName, serializedInput);
        }
    }
}
