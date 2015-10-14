CREATE TABLE [dbo].[ResourceTypes]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] VARCHAR(20) NOT NULL
	CONSTRAINT [CK_ResourceTypes_Name_Trim] CHECK ([Name] = ltrim([Name]) AND [Name] = rtrim([Name])), 
    CONSTRAINT [CK_ResourceTypes_Name_BadChars] CHECK ([Name] = replace([Name], char((9)), 'x') AND [Name] = replace([Name], char((10)), 'x') AND [Name] = replace([Name], char((13)), 'x')), 
)

GO

CREATE UNIQUE INDEX [IX_ResourceTypes_Name] ON [dbo].[ResourceTypes] ([Name])

GO

CREATE TRIGGER [dbo].[Trigger_ResourceTypes_UpdateUserPivotViews]
    ON [dbo].[ResourceTypes]
    AFTER DELETE, INSERT, UPDATE
    AS
    BEGIN
		IF @@ROWCOUNT = 0
			RETURN

        SET NoCount ON
		
		exec DynamicUpdateUserPivotViews
    END