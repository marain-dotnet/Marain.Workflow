CREATE PROCEDURE [dbo].[GetWorkflowInstance]
	@workflowInstanceId nvarchar(50) NOT NULL
AS
	SELECT [SerializedInstance], [ETag] FROM [WorkflowInstance] WHERE [WorkflowInstanceId] = @workflowInstanceId;
RETURN 0
