// <copyright file="GlobalSuppressions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "RCS1229:Use async/await when necessary.", Justification = "You cannot use async/await with a durable function activity", Scope = "member", Target = "~M:Endjin.Workflow.Functions.MessagePreProcessingHost.Activities.CreateWorkflowActivity.RunAction(Microsoft.Azure.WebJobs.DurableActivityContext,Microsoft.Azure.WebJobs.ExecutionContext,Microsoft.Extensions.Logging.ILogger)~System.Threading.Tasks.Task")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "RCS1229:Use async/await when necessary.", Justification = "You cannot use async/await with a durable function activity", Scope = "member", Target = "~M:Endjin.Workflow.Functions.MessagePreProcessingHost.Activities.ProcessTriggerActivity.RunAction(Microsoft.Azure.WebJobs.DurableActivityContext,Microsoft.Azure.WebJobs.ExecutionContext,Microsoft.Extensions.Logging.ILogger)~System.Threading.Tasks.Task")]
