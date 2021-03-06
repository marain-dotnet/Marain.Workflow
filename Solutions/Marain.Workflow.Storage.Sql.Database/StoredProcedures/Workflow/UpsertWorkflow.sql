﻿-- Upserts a serialized workflow, using an ETag for consistency.
-- Returns 200 if the upsert succeeds, or 409 if the upsert fails due to a collision
-- No changes will be made in the event of a conflict
CREATE PROCEDURE [dbo].[UpsertWorkflow]
-- The id of the workflow to upsert
	@workflowId nvarchar(50),
-- The etag of the old version of the document (or NULL if this is a new instance)
	@etag nvarchar(50),
-- The etag of the version to be inserted
	@newetag nvarchar(50),
-- The serialized workflow
	@serializedWorkflow nvarchar(MAX)
AS

DECLARE @id INT;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE

BEGIN TRANSACTION 

--Determine if we can insert or update
IF @etag IS NULL
BEGIN
	-- We are expecting this to be an insert, so verify that there is no existing record for this workflow ID
	IF EXISTS (SELECT Id FROM [Workflow] WHERE [WorkflowId] = @workflowId)
	BEGIN
		-- Conflict
		ROLLBACK TRANSACTION
		RETURN 409;
	END

	-- All is good, so insert the record and stash away its identity
	INSERT INTO [Workflow]([WorkflowId], SerializedWorkflow, ETag) VALUES (@workflowId, @serializedWorkflow, @newetag);
	SET @id = @@IDENTITY;
END
ELSE
BEGIN	
	-- We are expecting this to be an update, so ensure that a record exists with the expected ETag
	IF NOT EXISTS (SELECT Id FROM [Workflow] WHERE [WorkflowId] = @workflowId AND [ETag] = @etag)
	BEGIN
		-- Conflict
		ROLLBACK TRANSACTION
		RETURN 412;
	END

	-- All is good, so update the record, and set the new etag and instance value
	UPDATE [Workflow] SET @id = [Id], [ETag] = @newetag, [SerializedWorkflow] = @serializedWorkflow WHERE [WorkflowId] = @workflowId AND [ETag] = @etag;
END

-- Everyone's happy, so commit the transaction and return 200 (We are borrowing the HTTP error codes here for ease)
COMMIT TRANSACTION;
RETURN 200;