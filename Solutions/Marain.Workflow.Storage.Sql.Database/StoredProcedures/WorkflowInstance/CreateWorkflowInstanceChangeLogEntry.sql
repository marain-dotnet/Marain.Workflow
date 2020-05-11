-- Inserts a change log entry for a workflow instance.
-- Returns 200 if the upsert succeeds, 409 if the logId is already in use
CREATE PROCEDURE [dbo].[CreateWorkflowInstanceChangeLogEntry]
-- The id of the workflow instance to upsert
	@logId nvarchar(50),
-- The id of the workflow instance to upsert
	@workflowInstanceId nvarchar(50),
-- The serialized trigger instance, or null
	@serializedTrigger nvarchar(MAX) NULL,
-- The serialized workflow instance
	@serializedInstance nvarchar(MAX)
AS
DECLARE @id INT;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE

BEGIN TRANSACTION 

BEGIN
	-- We are expecting this to be an insert, so verify that there is no existing record for this workflow instance ID
	IF EXISTS (SELECT Id FROM [WorkflowInstanceChangeLog] WHERE [LogId] = @logId)
	BEGIN
		-- Conflict
		ROLLBACK TRANSACTION
		RETURN 409;
	END

	-- All is good, so insert the record and stash away its identity
	INSERT INTO [WorkflowInstanceChangeLog]([LogId], [WorkflowInstanceId], [SerializedInstance], [SerializedTrigger]) VALUES (@logId, @workflowInstanceId, @serializedInstance, @serializedTrigger);
	SET @id = @@IDENTITY;
END

-- Everyone's happy, so commit the transaction and return 200 (We are borrowing the HTTP error codes here for ease)
COMMIT TRANSACTION;
RETURN 200;