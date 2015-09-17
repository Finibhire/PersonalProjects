CREATE TABLE [dbo].[SellOrders]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] INT NOT NULL, 
    [ResourceTypeId] INT NOT NULL, 
    [ResourceSellAmount] INT NOT NULL, 
    [ResourceFilledAmount] INT NOT NULL DEFAULT 0, 
    [CurrencyTypeId] TINYINT NOT NULL, 
    [CurrencyPerResource] DECIMAL(38, 9) NOT NULL, 
    CONSTRAINT [CK_SellOrders_ResourceRequestAmount] CHECK ([ResourceSellAmount] > 0 and [ResourceSellAmount] > [ResourceFilledAmount]), 
    CONSTRAINT [CK_SellOrders_ResourceFilledAmount] CHECK ([ResourceFilledAmount] >= 0 and [ResourceFilledAmount] < [ResourceSellAmount]), 
    CONSTRAINT [CK_SellOrders_CurrencyPerResource] CHECK ([CurrencyPerResource] >= 0), 
    CONSTRAINT [FK_SellOrders_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]), 
    CONSTRAINT [FK_SellOrders_ResourceTypes] FOREIGN KEY ([ResourceTypeId]) REFERENCES [ResourceTypes]([Id]),
    CONSTRAINT [FK_SellOrders_CurrencyTypes] FOREIGN KEY ([CurrencyTypeId]) REFERENCES [CurrencyTypes]([Id])
)
