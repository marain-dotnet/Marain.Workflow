-- Gets the total number of workflow instances whose interests have a match for the given subjects
CREATE PROCEDURE [dbo].[GetMatchingWorkflowInstanceCountForSubjects]
-- A list of the subjects to match to the workflow's interests
	@subjects WorkflowInstanceInterestTableType READONLY
AS
	SELECT COUNT(DISTINCT [WorkflowInstance].Id) FROM [WorkflowInstance]
		JOIN [WorkflowInstanceInterest] ON [WorkflowInstance].Id = [WorkflowInstanceInterest].WorkflowInstance
	WHERE [WorkflowInstanceInterest].Interest IN (SELECT Interest FROM @subjects)
RETURN 0
