CREATE PROCEDURE [dbo].[GetWorkflow]
	@workflowId nvarchar(50) NOT NULL
AS
	SELECT [SerializedWorkflow], [Etag] FROM [Workflow] WHERE [WorkflowId] = @workflowId;
RETURN 0
