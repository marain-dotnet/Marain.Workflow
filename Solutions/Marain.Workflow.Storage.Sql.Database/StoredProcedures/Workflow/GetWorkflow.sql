CREATE PROCEDURE [dbo].[GetWorkflow]
	@workflowId nvarchar(50)
AS
	SELECT [SerializedWorkflow], [ETag] FROM [Workflow] WHERE [WorkflowId] = @workflowId;
RETURN 0
