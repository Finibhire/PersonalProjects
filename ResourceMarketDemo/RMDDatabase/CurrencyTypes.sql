CREATE TABLE [dbo].[CurrencyTypes]
(
	[Id] TINYINT NOT NULL PRIMARY KEY, 
    [Name] VARCHAR(20) NOT NULL,
    [MaxScale] TINYINT NOT NULL DEFAULT 0,
	CONSTRAINT [CK_CurrencyTypes_Name_Trim] CHECK ([Name] = ltrim([Name]) AND [Name] = rtrim([Name])), 
    CONSTRAINT [CK_CurrencyTypes_Name_BadChars] CHECK ([Name] = replace([Name], char((9)), 'x') AND [Name] = replace([Name], char((10)), 'x') AND [Name] = replace([Name], char((13)), 'x')), 
    CONSTRAINT [CK_CurrencyTypes_MaxScale_Range] CHECK ([MaxScale] >= 0 AND [MaxScale] <= 9)

)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The maximum number of decimal points that should be saved for a currency',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CurrencyTypes',
    @level2type = N'COLUMN',
    @level2name = N'MaxScale'
GO

CREATE UNIQUE INDEX [IX_CurrencyTypes_Name] ON [dbo].[CurrencyTypes] ([Name])

GO

CREATE TRIGGER [dbo].[Trigger_CurrencyTypes_UpdateUserPivotViews]
    ON [dbo].[CurrencyTypes]
    AFTER DELETE, INSERT, UPDATE
    AS
    BEGIN
		IF @@ROWCOUNT = 0
			RETURN 

        SET NoCount ON
		
		exec DynamicUpdateUserPivotViews
    END