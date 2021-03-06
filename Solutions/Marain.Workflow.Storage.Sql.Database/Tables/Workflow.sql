﻿CREATE TABLE [dbo].[Workflow]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [WorkflowId] NVARCHAR(50) NOT NULL UNIQUE, 
    [SerializedWorkflow] NVARCHAR(MAX) NOT NULL, 
    [ETag] NVARCHAR(50) NOT NULL 
)

GO 

CREATE INDEX [IX_Workflow_WorkflowId] ON [dbo].[Workflow] ([WorkflowId])
