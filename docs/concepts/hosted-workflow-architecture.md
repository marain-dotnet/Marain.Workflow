# Hosted workflow architecture

## Introduction

Whilst it is possible to use the Marain workflow engine in-process by simply consuming the Marain.Abstractions package and the appropriate storage packages, we strongly recommend it is deployed as standalone services using the provided Azure Function apps. Using these functions gives you a number of extremely useful features out of the box.

- Scalability: deploying the functions apps using the consumption-based hosting plan allows scaling out trigger processing across up to 200 instances.
- High-volume message ingestion: If it is necessary to ingest extremely high volumes of triggers, but there is less need for them to be processed quickly, a separate ingestion endpoint can be used which uses Event Hubs to ingest and queue messages for processing *.
-


\* This function hasn't yet been added into Marain.Workflow, but can be brought in when we need it. There's [a GitHub issue tracking this work](https://github.com/marain-dotnet/Marain.Workflow/issues/102).