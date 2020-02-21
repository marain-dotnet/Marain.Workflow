CREATE PROCEDURE [dbo].[GetWorkflowInstance]
	@workflowInstanceId nvarchar(50)
AS
	SELECT [SerializedInstance], [ETag] FROM [WorkflowInstance] WHERE [WorkflowInstanceId] = @workflowInstanceId;
RETURN 0
