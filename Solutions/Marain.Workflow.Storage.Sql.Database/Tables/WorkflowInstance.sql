CREATE TABLE [dbo].[WorkflowInstance]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [WorkflowInstanceId] NVARCHAR(50) NOT NULL UNIQUE, 
    [SerializedInstance] NVARCHAR(MAX) NOT NULL, 
    [ETag] NVARCHAR(50) NOT NULL 
)

GO

CREATE INDEX [IX_WorkflowInstance_WorkflowInstanceId] ON [dbo].[WorkflowInstance] ([WorkflowInstanceId]) 

GO

CREATE INDEX [IX_WorkflowInstance_WorkflowInstanceAndEtag] ON [dbo].[WorkflowInstance] ([WorkflowInstanceId], [ETag])
