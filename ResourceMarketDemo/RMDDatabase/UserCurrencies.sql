CREATE TABLE [dbo].[UserCurrencies]
(
	[UserId] INT NOT NULL , 
    [CurrencyId] TINYINT NOT NULL, 
    [OnHand] DECIMAL(38, 9) NOT NULL DEFAULT 0, 
    PRIMARY KEY ([UserId], [CurrencyId]), 
    CONSTRAINT [FK_UserCurrencies_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_UserCurrencies_CurrencyTypes] FOREIGN KEY ([CurrencyId]) REFERENCES [CurrencyTypes]([Id])
)
