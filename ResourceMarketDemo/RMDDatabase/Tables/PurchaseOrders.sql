CREATE TABLE [dbo].[PurchaseOrders]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] INT NOT NULL, 
    [ResourceTypeId] INT NOT NULL, 
    [ResourceRequestAmount] INT NOT NULL, 
    [ResourceFilledAmount] INT NOT NULL DEFAULT 0, 
    [CurrencyTypeId] TINYINT NOT NULL, 
    [CurrencyPerResource] FLOAT(53) NOT NULL, 
    CONSTRAINT [CK_PurchaseOrders_ResourceRequestAmount] CHECK ([ResourceRequestAmount] > 0 and [ResourceRequestAmount] > [ResourceFilledAmount]), 
    CONSTRAINT [CK_PurchaseOrders_ResourceFilledAmount] CHECK ([ResourceFilledAmount] >= 0 and [ResourceFilledAmount] < [ResourceRequestAmount]), 
    CONSTRAINT [CK_PurchaseOrders_CurrencyPerResource_NonNegative] CHECK ([CurrencyPerResource] >= 0), 
    CONSTRAINT [CK_PurchaseOrders_CurrencyPerResource_SigFigs] CHECK (CurrencyPerResource = dbo.g_OrderSigFigsFloor(CurrencyPerResource)),
    CONSTRAINT [FK_PurchaseOrders_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]), 
    CONSTRAINT [FK_PurchaseOrders_ResourceTypes] FOREIGN KEY ([ResourceTypeId]) REFERENCES [ResourceTypes]([Id]),
    CONSTRAINT [FK_PurchaseOrders_CurrencyTypes] FOREIGN KEY ([CurrencyTypeId]) REFERENCES [CurrencyTypes]([Id])
)

GO

CREATE TRIGGER [dbo].[Trigger_PurchaseOrders_INSERT]
    ON [dbo].[PurchaseOrders]
    FOR INSERT
    AS
    BEGIN
        SET NoCount ON

		if not exists(select Id from inserted)
			return

		
		--insert 0's into UserCurrencies where nessicary

		--Update the UserCurrencies.OnHand to reflect the amount pulled out of their account to 
		--create these PurchaseOrders.  

		insert into UserCurrencies 
		(
			UserId,
			CurrencyTypeId,
			OnHand
		)
		select
			i.UserId as UserId,
			i.CurrencyTypeId as CurrencyTypeId,
			cast(0 as decimal(38,9)) as OnHand
		from
			inserted i
			left join UserCurrencies uc on uc.UserId = i.UserId and uc.CurrencyTypeId = i.CurrencyTypeId
		where
			uc.UserId is null
		group by
			i.UserId, i.CurrencyTypeId

		
		update uc
		set
			uc.OnHand = uc.OnHand - i.TotalCurrencyCost
		from
			UserCurrencies uc
			inner join (
				select
					i2.UserId,
					i2.CurrencyTypeId,
					sum(dbo.fRoundDecimalUp(i2.CurrencyPerResource * cast(i2.ResourceRequestAmount - i2.ResourceFilledAmount as decimal(38,9)), ct.MaxScale)) as TotalCurrencyCost 
				from 
					UserCurrencies uc2
					inner join inserted i2 on uc2.UserId = i2.UserId and uc2.CurrencyTypeId = i2.CurrencyTypeId
					inner join CurrencyTypes ct on uc2.CurrencyTypeId = ct.Id
				group by
					i2.UserId, i2.CurrencyTypeId
			) i on uc.UserId = i.UserId and uc.CurrencyTypeId = i.CurrencyTypeId
    END