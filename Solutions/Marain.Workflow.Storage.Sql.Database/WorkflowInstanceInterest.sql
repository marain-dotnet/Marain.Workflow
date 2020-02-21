CREATE TABLE [dbo].[WorkflowInstanceInterest]
(
	[Id] INT NOT NULL , 
    [Interest] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [PK_WorkflowInstanceInterest] PRIMARY KEY ([Id], [Interest]), 
    CONSTRAINT [FK_WorkflowInstanceInterest_WorkflowInstance] FOREIGN KEY ([Id]) REFERENCES [WorkflowInstance](Id)
)
