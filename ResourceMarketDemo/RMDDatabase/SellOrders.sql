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

GO

CREATE TRIGGER [dbo].[Trigger_SellOrders_INSERT]
    ON [dbo].[SellOrders]
    FOR INSERT
    AS
    BEGIN
        SET NoCount ON

		if not exists(select * from inserted)
			return

		
		--insert 0's into UserCurrencies where nessicary

		--Update the UserResources.OnHand to reflect the amount pulled out of their account to 
		--create these SellOrders.  

		insert into UserResources 
		(
			UserId,
			ResourceTypeId,
			OnHand
		)
		select
			i.UserId as UserId,
			i.ResourceTypeId as ResourceTypeId,
			cast(0 as decimal(38,9)) as OnHand
		from
			inserted i
			left join UserResources ur on ur.UserId = i.UserId and ur.ResourceTypeId = i.ResourceTypeId
		where
			ur.UserId is null
		group by
			i.UserId, i.ResourceTypeId

		
		update ur
		set
			ur.OnHand = ur.OnHand - i.TotalSellAmount
		from
			UserResources ur
			inner join (
				select
					i2.UserId,
					i2.ResourceTypeId,
					sum(i2.ResourceSellAmount) as TotalSellAmount 
				from 
					inserted i2
				group by
					i2.UserId, i2.ResourceTypeId
			) i on ur.UserId = i.UserId and ur.ResourceTypeId = i.ResourceTypeId
    END