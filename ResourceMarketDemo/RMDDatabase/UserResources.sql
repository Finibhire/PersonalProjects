CREATE TABLE [dbo].[UserResources]
(
	[UserId] INT NOT NULL , 
    [ResourceTypeId] INT NOT NULL, 
    [OnHand] BIGINT NOT NULL DEFAULT 0, 
    PRIMARY KEY ([UserId], [ResourceTypeId]), 
    CONSTRAINT [CK_UserResources_OnHand_NonNegative] CHECK ([OnHand] >= 0), 
    CONSTRAINT [FK_UserResources_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_UserResources_ResourceTypes] FOREIGN KEY ([ResourceTypeId]) REFERENCES [ResourceTypes]([Id])
)
