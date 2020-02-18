-- Gets a page of workflow instances whose interests have a match for the given subjects
CREATE PROCEDURE [dbo].[GetMatchingWorkflowInstancesForSubjects]
-- A list of the subjects to match to the workflow's interests
	@subjects WorkflowInstanceInterestTableType NOT NULL READONLY,
-- The maximum number of rows to return
	@pageSize int NOT NULL,
-- The zero-based index of the page of rows to return
	@pageIndex int NOT NULL
AS
	SELECT DISTINCT [WorkflowInstance].WorkflowInstanceId FROM [WorkflowInstance]
		JOIN [WorkflowInstanceInterest] ON [WorkflowInstance].Id = [WorkflowInstanceInterest].Id
	WHERE [WorkflowInstanceInterest].Interest IN (SELECT Interest FROM @subjects)
	ORDER BY [WorkflowInstance].Id
	OFFSET (@pageSize * @pageIndex) ROWS
	FETCH NEXT @pageSize ROWS ONLY;
RETURN 0
