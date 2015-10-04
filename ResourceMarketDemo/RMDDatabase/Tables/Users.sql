CREATE TABLE [dbo].[Users]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserName] VARCHAR(20) NOT NULL, 
    CONSTRAINT [CK_Users_UserName_Trim] CHECK ([UserName] = ltrim([UserName]) AND [UserName] = rtrim([UserName])), 
    CONSTRAINT [CK_Users_UserName_BadChars] CHECK ([UserName] = replace([UserName], char((9)), 'x') AND [UserName] = replace([UserName], char((10)), 'x') AND [UserName] = replace([UserName], char((13)), 'x'))
)

GO

CREATE UNIQUE INDEX [IX_Users_UserName] ON [dbo].[Users] ([UserName])
