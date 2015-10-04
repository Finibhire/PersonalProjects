CREATE TABLE [dbo].[UserCurrencies]
(
	[UserId] INT NOT NULL , 
    [CurrencyTypeId] TINYINT NOT NULL, 
    [OnHand] DECIMAL(38, 9) NOT NULL DEFAULT 0, 
    PRIMARY KEY ([UserId], [CurrencyTypeId]), 
    CONSTRAINT [FK_UserCurrencies_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_UserCurrencies_CurrencyTypes] FOREIGN KEY ([CurrencyTypeId]) REFERENCES [CurrencyTypes]([Id])
)
