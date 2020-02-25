CREATE TABLE [dbo].[WorkflowInstanceInterest]
(
	[WorkflowInstance] INT NOT NULL , 
    [Interest] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [PK_WorkflowInstanceInterest] PRIMARY KEY ([WorkflowInstance], [Interest]), 
    CONSTRAINT [FK_WorkflowInstanceInterest_WorkflowInstance] FOREIGN KEY ([WorkflowInstance]) REFERENCES [WorkflowInstance](Id)
)
