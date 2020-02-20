CREATE PROCEDURE [dbo].[GetWorkflow]
	@workflowId nvarchar(50)
AS
	SELECT [SerializedWorkflow], [Etag] FROM [Workflow] WHERE [WorkflowId] = @workflowId;
RETURN 0
