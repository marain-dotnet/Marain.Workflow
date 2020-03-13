# Storage

## Overview

The core workflow engine defines an abstraction for storage of both workflows and workflow instances. The specific implementations of these can vary independently.

Workflow storage is the simpler of the two, only requiring the ability to store and retrieve workflows by Id. Instance storage is more complex, requiring the ability to retrieve pages of workflow instances (or a count of instances) whose list of interests contains at least one match from a supplied list of subjects.

Two possible storage mechanisms are available in the solution: CosmosDB and SQL Server/Azure SQL.

// TODO - how do we switch between the two? Need an ADR for this.
