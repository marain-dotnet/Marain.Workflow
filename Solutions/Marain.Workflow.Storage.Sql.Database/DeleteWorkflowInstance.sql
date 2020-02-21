CREATE PROCEDURE [dbo].[DeleteWorkflowInstance]
	@workflowInstanceId nvarchar(50)
AS
	
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE

BEGIN TRANSACTION 

DECLARE @id INT;

SELECT @id = Id FROM [WorkflowInstance] WHERE [WorkflowInstanceId] = @workflowInstanceId

-- We are expecting this to exist as it is a DELETE, so ensure that a record exists with the expected ETag
IF @id IS NULL
BEGIN
	-- Not found
	ROLLBACK TRANSACTION
	RETURN 404;
END

-- Delete the existing interests
DELETE FROM [WorkflowInstanceInterest] WHERE [Id] = @id;

-- And delete the record
DELETE FROM [WorkflowInstance] WHERE [Id] = @id;

-- Everyone's happy, so commit the transaction and return 200 (We are borrowing the HTTP error codes here for ease)
COMMIT TRANSACTION;
RETURN 200;
