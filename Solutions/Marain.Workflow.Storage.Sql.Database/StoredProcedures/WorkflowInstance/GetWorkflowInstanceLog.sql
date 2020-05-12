-- Gets a page of workflow instance logs for the given instance id
CREATE PROCEDURE [dbo].[GetWorkflowInstanceLog]
-- The Id of the instance to get
	@workflowInstanceId  nvarchar(50),
	@startingTimestamp bigint NULL,
-- The maximum number of rows to return
	@pageSize int,
-- The zero-based index of the page of rows to return
	@pageIndex int
AS
IF @startingTimestamp IS NULL
BEGIN
	SELECT [WorkflowInstanceChangeLog].WorkflowInstanceId, [WorkflowInstanceChangeLog].SerializedTrigger, [WorkflowInstanceChangeLog].SerializedInstance, [WorkflowInstanceChangeLog].Timestamp  FROM [WorkflowInstanceChangeLog]
	WHERE [WorkflowInstanceChangeLog].WorkflowInstanceId = @workflowInstanceId
	ORDER BY [WorkflowInstanceChangeLog].SequenceNumber
	OFFSET (@pageSize * @pageIndex) ROWS
	FETCH NEXT @pageSize ROWS ONLY;
END
ELSE
BEGIN
	SELECT [WorkflowInstanceChangeLog].WorkflowInstanceId, [WorkflowInstanceChangeLog].SerializedTrigger, [WorkflowInstanceChangeLog].SerializedInstance, [WorkflowInstanceChangeLog].Timestamp  FROM [WorkflowInstanceChangeLog]
	WHERE [WorkflowInstanceChangeLog].WorkflowInstanceId = @workflowInstanceId
		AND [WorkflowInstanceChangeLog].Timestamp >= @startingTimestamp
	ORDER BY [WorkflowInstanceChangeLog].SequenceNumber
	OFFSET (@pageSize * @pageIndex) ROWS
	FETCH NEXT @pageSize ROWS ONLY;
END

RETURN 0
