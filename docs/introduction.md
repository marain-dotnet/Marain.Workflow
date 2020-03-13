# Introduction

Marain Workflow is a highly scalable workflow service built on the .NET framework. It provides an underlying workflow engine and web-based APIs implementing a hosted version of the engine using Microsoft's Azure platform

## Background

Despite the fact that a large percentage of business applications represent some kind of workflow based system, this is rarely acknowledged. Instead, domain objects are given status properties, normally using boolean and enumable data types which, when used together, represent different states that an entity can be in.

When the underlying workflow changes, this often means changes in multiple areas of the system to ensure that everywhere bespoke state checking is used, it is still correct.

An alternative to this approach is model these systems as what they are: business processes that can be expressed as a workflow.

## What is a workflow?

A workflow is, at it's simplest, a collection of *states* that an entity can be in, and *transitions* between those entities.

These states and transitions are augmented with *actions*, things that happen as part of these state changes, and *conditions* which govern whether these changes are allowed to happen.

With these four simple constructs, it is straightforward to model business processes in their entirety, accurately capturing the reasons that an entity's state changes over time.

## Why use a workflow engine?

When building a system that is essentially driven by workflow, a common approach is make the workflow implementation part of the domain model. This can be done using something like the [GoF State pattern](https://en.wikipedia.org/wiki/State_pattern).

A more flexible approach is to use a workflow engine to model the workflow and manage instances of it. This allows the workflow to be modified independently of the code that implements the different conditions and actions that are required as workflow instances move through the different states.

It also results in much simpler code; defining your workflow's state changes in terms of the conditions and actions that are required naturally leads to those conditions and actions being implemented as small, reusable and easily testable blocks of code that are brought together by the workflow engine.

## Where is it appropriate to use a workflow engine?

Business applications tend to fall into categories, from long running processes that require minimal or no user interaction, to highly interactive UI-based tools that allow users to carry out day-to-day business activities.

On closer inspection, a high percentage of business systems essentially workflow-driven data management tools, even if that is never explicitly acknowledged.

While it can be difficult to justify adopting a workflow engine when building these systems, the real value comes when the underlying business processes - the workflows - need to be modified. Since the workflows themselves are modelled within the system, they are the focal point for changes; should new conditions or actions be needed to support the changes, they are straightforward to add to the codebase.

Using a workflow engine also helps answer questions around how to migrate from one version of a workflow to the next - for example, it is straightforward to have two versions of a workflow exist side-by-side in the workflow engine, with existing workflow instance continuing to use an previous version of a workflow and new instances based on a new version.


## Why use *this* workflow engine?

Marain.Workflow was built from the ground up to address some of the problems commonly found in other workflow engines.

Foremost in these is that of scalability. The hosting APIs provided with the workflow engine are implemented using serverless functions running in Windows Azure, enabling messages to be ingested and processed at extremely high rates.

In addition, it is technology agnostic. Whilst it's built using C# and the provided hosting is targetted at Microsoft Azure, conditions and actions can be implemented in any other language as long as they are exposed via an HTTP endpoint that can accept calls from the workflow engine.
