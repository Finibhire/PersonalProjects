CREATE TABLE [dbo].[MarketSales]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SellerUserId] INT NOT NULL, 
    [BuyerUserId] INT NOT NULL, 
    [ResourceTypeId] INT NOT NULL, 
    [ResourcesSoldAmount] INT NOT NULL, 
    [CurrencyTypeId] TINYINT NOT NULL, 
    [TotalCurrencyCost] DECIMAL(38, 9) NOT NULL, 
    [TimeStamp] DATETIME NOT NULL DEFAULT getdate(), 
    CONSTRAINT [FK_MarketSales_Users_Seller] FOREIGN KEY ([SellerUserId]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_MarketSales_Users_Buyer] FOREIGN KEY ([BuyerUserId]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_MarketSales_ResourceTypes] FOREIGN KEY ([ResourceTypeId]) REFERENCES [ResourceTypes]([Id]),
    CONSTRAINT [FK_MarketSales_CurrencyTypes] FOREIGN KEY ([CurrencyTypeId]) REFERENCES [CurrencyTypes]([Id])
)

GO

CREATE TRIGGER [dbo].[Trigger_MarketSales_INSERT]
    ON [dbo].[MarketSales]
    FOR INSERT
    AS
    BEGIN
        SET NoCount ON

		if not exists(select * from inserted)
			return

		--insert 0's into rows that we need in UserCurrencies and UserResources

		--update the following working totals to reflect these changes
		--   Buyer's UserResources.OnHand 
		--   Seller's UserCurrencies.OnHand

		--Seller's UserResources should already be deducted and same with Buyer's UserCurrencies
		--SellOrder.AmountFilled should be updated
		--though non-trigger logic as this trigger doesn't have the information to update that information.

		insert into UserResources 
		(
			UserId,
			ResourceTypeId,
			OnHand
		)
		select 
			i.BuyerUserId as UserId,
			i.ResourceTypeId as ResourceId,
			cast(0 as int) as OnHand
		from
			inserted i
			left join UserResources ur on ur.UserId = i.BuyerUserId and ur.ResourceTypeId = i.ResourceTypeId
		where
			i.BuyerUserId is null
		group by
			i.BuyerUserId, i.ResourceTypeId

		insert into UserCurrencies 
		(
			UserId,
			CurrencyTypeId,
			OnHand
		)
		select
			i.BuyerUserId as UserId,
			i.CurrencyTypeId as CurrencyTypeId,
			cast(0 as decimal(38,9)) as OnHand
		from
			inserted i
			left join UserCurrencies uc on uc.UserId = i.BuyerUserId and uc.CurrencyTypeId = i.CurrencyTypeId
		where
			uc.UserId is null
		group by
			i.BuyerUserId, i.CurrencyTypeId
		
		insert into UserCurrencies 
		(
			UserId,
			CurrencyTypeId,
			OnHand
		)
		select
			i.BuyerUserId as UserId,
			i.CurrencyTypeId as CurrencyTypeId,
			cast(0 as decimal(38,9)) as OnHand
		from
			inserted i
			left join UserCurrencies uc on uc.UserId = i.SellerUserId and uc.CurrencyTypeId = i.CurrencyTypeId
		where
			uc.UserId is null
		group by
			i.BuyerUserId, i.CurrencyTypeId

		update ur
		set
			ur.OnHand = ur.OnHand + i.TotalResourcesSoldAmount
		from
			UserResources ur
			inner join (
				select
					i2.BuyerUserId as UserId,
					i2.ResourceTypeId,
					sum(i2.ResourcesSoldAmount) as TotalResourcesSoldAmount
				from 
					UserResources ur2
					inner join inserted i2 on ur2.UserId = i2.BuyerUserId and ur2.ResourceTypeId = i2.ResourceTypeId
				group by
					i2.BuyerUserId, i2.ResourceTypeId
			) i on ur.UserId = i.UserId and ur.ResourceTypeId = i.ResourceTypeId
			
		--update uc
		--set
		--	uc.OnHand = uc.OnHand - i.TotalCurrencyCost
		--from
		--	UserCurrencies uc
		--	inner join (
		--		select
		--			i2.BuyerUserId,
		--			i2.CurrencyTypeId,
		--			sum(i2.TotalCurrencyCost) as TotalCurrencyCost
		--		from 
		--			UserCurrencies uc2
		--			inner join inserted i2 on uc2.UserId = i2.BuyerUserId and uc2.CurrencyTypeId = i2.CurrencyTypeId
		--		group by
		--			i2.BuyerUserId, i2.CurrencyTypeId
		--	) i on uc.UserId = i.BuyerUserId and uc.CurrencyTypeId = i.CurrencyTypeId

		update uc
		set
			uc.OnHand = uc.OnHand + i.TotalCurrencyCost
		from
			UserCurrencies uc
			inner join (
				select
					i2.SellerUserId,
					i2.CurrencyTypeId,
					sum(i2.TotalCurrencyCost) as TotalCurrencyCost
				from 
					UserCurrencies uc2
					inner join inserted i2 on uc2.UserId = i2.SellerUserId and uc2.CurrencyTypeId = i2.CurrencyTypeId
				group by
					i2.SellerUserId, i2.CurrencyTypeId
			) i on uc.UserId = i.SellerUserId and uc.CurrencyTypeId = i.CurrencyTypeId
		
    END