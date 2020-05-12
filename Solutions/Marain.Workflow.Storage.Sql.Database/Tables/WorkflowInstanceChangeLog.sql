CREATE TABLE [dbo].[WorkflowInstanceChangeLog]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [LogId] NVARCHAR(50) NOT NULL UNIQUE, 
    [WorkflowInstanceId] NVARCHAR(50) NOT NULL, 
    [SerializedTrigger] NVARCHAR(MAX), 
    [SerializedInstance] NVARCHAR(MAX) NOT NULL, 
    [SequenceNumber] ROWVERSION NOT NULL, 
    [Timestamp] INT NOT NULL DEFAULT DATEDIFF(second,{d '1970-01-01'},CURRENT_TIMESTAMP) 
)

GO

CREATE INDEX [IX_WorkflowInstanceChangeLog_WorkflowInstanceId] ON [dbo].[WorkflowInstance] ([WorkflowInstanceId]) 