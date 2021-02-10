# Event store selection

## Status

Proposed

## Context

As described in ADR 0001, we intend to re-implement the workflow engine using an event sourcing approach for workflow instances.

This ADR covers selection of an event store implementation; it lists the candidates and reasoning for the final selection.

## Candidates

When looking at candidates, there are the following key points to consider:
1. Any barriers to integration; will it work in the Azure Functions runtime?
2. What storage options are available. We want data to be stored in one of Azure's cloud-native services such as table storage or CosmosDb.
3. What support is available for event publishing? We need to be able to publish events out of the workflow engine to a service like Azure's Event Grid.
4. Scalability - what restrictions will the candidate put on our ability to scale the service? This is closely related to the underlying storage used.
5. Is it proven to work in a production environment?
6. If open source, is it actively maintained?
7. Are there any cost implications to adoption?

The candidates are:
- NEventStore: https://github.com/NEventStore/NEventStore
- Corvus.EventStore: https://github.com/corvus-dotnet/Corvus.EventStore
- EventStore DB: https://www.eventstore.com/
- JKang.EventSourcing: https://github.com/jacqueskang/EventSourcing

| | NEventStore | Corvus.EventStore | EventStore DB | JKang.EventSourcing |
|-|-|-|-|-|
| Barriers to integration | Can't use latest version due to dependency on Microsoft.Extensions.Logging.Abstractions meaning it doesn't work in the functions runtime.| None | Deployed as a standalone service - would increase the complexity of a Marain deployment. We'd also be tying Marain to a third party commercial product. | Lack of confidence. |
| Storage| No support for Table/Blob/CosmosDb. However, there is an implementation for MongoDb which we can use with Cosmos | Table or CosmosDb. | Native | CosmosDb, anything supported by EfCore. |
| Publishing | Includes "PollingClient" to reliably publish each event at least once. | Implementation in progress. | Native | Not implemented in framework. |
| Scalability | Limited due to inability to partition the event stream. | High | Docs discuss partitioning, vertical scaling, but hosted option is in preview with no published SLA. | Uncertain. |
| Production use | Yes | No | Yes | Uncertain. |
| Maintained | Kept up to date with latest framework versions. Appears to have only one maintainer at the moment. | Not yet | Yes | Sporadic; has only ever had one developer.|
| Cost | N/A | N/A | Smallest hosted Azure instance is ~£30ppm | N/A |

## Decision

None of the candidates meet all our criteria in terms of scalability and storage. However, in the short term we can accommodate some of the drawbacks of our selection as we work to address them, either via pull requests to the selected repository or by continuing to work on the Corvus.EventStore project.

**Corvus.EventStore** is too big a risk at present because it is is not production ready. In order to use it we'd need to complete the implementation of event publishing from the all-stream, and then we would likely encounter the normal edge cases and issues one would expect when putting a project of this nature into production use for the first time. As such, it's too high a risk to adopt at the moment.

**EventStore DB** looks to be a good offering, but we do not want to tightly couple Marain to third party commercial solutions that would make it more difficult to implement. As such, we're discounting this option.

**JKang.EventSourcing** is a relatively modern implementation but is not viable in its current state; it is missing a major component (event publishing) and our experiences implementing this in Corvus.EventStore suggest that adding it will not be a trivial task.

As such, we will implement against NEventStore in the first instance. Since the majority of the code change needed in the workflow engine follows standard patterns for Event Sourcing, we would not expect it to be a huge technical challenge to migrate to another option (e.g. Corvus.EventSourcing) in future if required.

## Consequences

We will look to implement using the MongoDb persistence provider to connect to CosmosDb. However, we need to further investigate how this would work with the tenancy model that Marain uses throughout.
