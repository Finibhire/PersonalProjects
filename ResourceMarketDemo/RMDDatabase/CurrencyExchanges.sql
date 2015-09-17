CREATE TABLE [dbo].[CurrencyExchanges]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] INT NOT NULL, 
    [SourceCurrencyTypeId] TINYINT NOT NULL, 
    [DestinationCurrencyTypeId] TINYINT NOT NULL, 
    [SourceAmount] DECIMAL(38, 9) NOT NULL, 
    [DestinationAmount] DECIMAL(38, 9) NOT NULL, 
    [TimeStamp] DATETIME NOT NULL DEFAULT getdate(), 
    CONSTRAINT [CK_CurrencyExchanges_SourceDestCurDiff] CHECK ([SourceCurrencyTypeId] <> [DestinationCurrencyTypeId]), 
    CONSTRAINT [FK_CurrencyExchanges_CrurrencyTypes_Source] FOREIGN KEY ([SourceCurrencyTypeId]) REFERENCES [CurrencyTypes]([Id]), 
    CONSTRAINT [FK_CurrencyExchanges_CrurrencyTypes_Destination] FOREIGN KEY ([DestinationCurrencyTypeId]) REFERENCES [CurrencyTypes]([Id])
)

GO

CREATE TRIGGER [dbo].[Trigger_CurrencyExchanges_INSERT]
    ON [dbo].[CurrencyExchanges]
    FOR INSERT
    AS
    BEGIN
        SET NoCount ON

		if not exists(select * from inserted)
			return

		--insert zero OnHand values into UserCurrencies where needed
		insert into UserCurrencies
		select 
			i.UserId as UserId,
			i.SourceCurrencyTypeId as CurrencyTypeId,
			cast(0 as decimal(38,9)) as OnHand
		from
			inserted i
			left join UserCurrencies uc on i.UserId = uc.UserId and i.SourceCurrencyTypeId = uc.CurrencyTypeId
		where
			uc.UserId is null
		group by
			i.UserId, i.SourceCurrencyTypeId

		insert into UserCurrencies
		select 
			i.UserId as UserId,
			i.DestinationCurrencyTypeId as CurrencyTypeId,
			cast(0 as decimal(38,9)) as OnHand
		from
			inserted i
			left join UserCurrencies uc on i.UserId = uc.UserId and i.DestinationCurrencyTypeId = uc.CurrencyTypeId
		where
			uc.UserId is null
		group by
			i.UserId, i.DestinationCurrencyTypeId

		--select * from inserted --debug

		--update UserCurrencies to reflect the changes in CurrencyExchanges
		update uc
		set 
			uc.OnHand = uc.OnHand - dest.TotalSourceAmount
		from (
			select 
				i.UserId,
				i.SourceCurrencyTypeId,
				sum(i.SourceAmount) as TotalSourceAmount
			from
				inserted i
				inner join UserCurrencies uc2
					on i.UserId = uc2.UserId and i.SourceCurrencyTypeId = uc2.CurrencyTypeId
			group by i.UserId, i.SourceCurrencyTypeId
			) as dest
			inner join UserCurrencies uc on dest.UserId = uc.UserId and dest.SourceCurrencyTypeId = uc.CurrencyTypeId

		--select --debug
		--	i.UserId,
		--	i.SourceCurrencyTypeId,
		--	sum(i.SourceAmount) as TotalSourceAmount
		--from
		--	inserted i
		--	inner join UserCurrencies uc2
		--		on i.UserId = uc2.UserId and i.SourceCurrencyTypeId = uc2.CurrencyTypeId
		--group by i.UserId, i.SourceCurrencyTypeId


		update uc
		set 
			uc.OnHand = uc.OnHand + dest.TotalDestinationAmount
		from (
			select 
				i.UserId,
				i.DestinationCurrencyTypeId,
				sum(i.DestinationAmount) as TotalDestinationAmount
			from
				inserted i
				inner join UserCurrencies uc2
					on i.UserId = uc2.UserId and i.DestinationCurrencyTypeId = uc2.CurrencyTypeId
			group by i.UserId, i.DestinationCurrencyTypeId
			) as dest
			inner join UserCurrencies uc on dest.UserId = uc.UserId and dest.DestinationCurrencyTypeId = uc.CurrencyTypeId
    END