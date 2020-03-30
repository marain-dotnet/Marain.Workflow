# Hosted workflow architecture

## Introduction

Whilst it is possible to use the Marain workflow engine in-process by simply consuming the Marain.Abstractions package and the appropriate storage packages, we strongly recommend it is deployed as standalone services using the provided Azure Function apps. Using these functions gives some extremely useful features out of the box.

- Scalability: deploying the functions apps using the consumption-based hosting plan allows scaling out trigger processing across up to 200 instances.
- High-volume message ingestion: If it is necessary to ingest extremely high volumes of triggers, but there is less need for them to be processed quickly, a separate ingestion endpoint can be used which uses Event Hubs to ingest and queue messages for processing \*.
- Flexibility to use the technology of your choice to implement actions and conditions.

## Architecture

```
                                      +-----------------+
      HTTP Request                    |                 |
      (triggers and                   | High traffic    |
      start instance  +---------------> ingestion       |
      requests)                       | endpoints       |
                                      |                 |
                                      +--------+--------+
                                               |
                                               | Messages
                                               |
                                      +--------v--------+
                                      |                 |
                                      | Azure Event Hub |
                                      |                 |
                                      +---------------+-+
                                                      |
                                                      | Messages
                                                      |
                             +-----------------+------v----------+
 HTTP Request                |                 |                 |
 (triggers and               | Standard        | Event hub       |
 start instance  +-----------> ingestion       | message         |
 requests)                   | endpoints       | ingestion       |
                             |                 |                 |
                             +-----------------+-----------------+
                             |                                   |
                             |                                   |
                             |        Message processing         |
                             |                                   |
                             |                                   |
                             +-----------------+-----------------+
                                               |
                                               | Requests
                                               |                        +-----------------+
+-----------------+                   +--------v--------+               |                 |
|                 |                   |                 +---------------> Workflow store  |
|  External       |                   | Workflow        |               |                 |
|  services       <-------------------+ engine          | Load/save     +-----------------+
|                 | Invoke conditions |                 |               |                 |
|                 | and actions       |                 +---------------> Workflow        |
+-----------------+                   +-----------------+               | instance store  |
                                                                        |                 |
                                                                        +-----------------+
```

## Security

All of the functions are secured using Azure Active Directory. The message processing function (middle) authenticates with the workflow engine using an Azure Managed Identity; similarly, the Workflow Engine can authenticate with external services using Managed Identity and AAD, or via a key in the target URL.

When sending triggers/start instance requests, regardless of whether the high traffic endpoint is used, authentication is still expected to take place via AAD. If using the provided client library, support for authentication using the Managed Identity of the host function or application is built in (further information on this can be found in the "How to" section).



\* This function hasn't yet been added into Marain.Workflow, but can be brought in when we need it. There's [a GitHub issue tracking this work](https://github.com/marain-dotnet/Marain.Workflow/issues/102).