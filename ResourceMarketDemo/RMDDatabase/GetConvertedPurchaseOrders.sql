CREATE PROCEDURE [dbo].[GetConvertedPurchaseOrders]
	@workingCurrency tinyint,
	@workingResource int
AS
	select 
		Id,
		UserId,
		isnull(ToBeFilledAmount, 0) as ToBeFilledAmount,
		OriginalCurrencyTypeId,
		OriginalCurrencyName,
		SourceMultiplier,
		isnull(ConvertedCurrencyPerResource, 0) as ConvertedCurrencyPerResource
	from
		(
			select 
				po.Id,
				po.UserId,
				po.ResourceRequestAmount - ResourceFilledAmount as ToBeFilledAmount,
				po.CurrencyTypeId as OriginalCurrencyTypeId,
				ct.Name as OriginalCurrencyName,
				isnull(cer.SourceMultiplier, 1) as SourceMultiplier,
				dbo.g_OrderSigFigsFloor(po.CurrencyPerResource / isnull(cer.SourceMultiplier, 1)) as ConvertedCurrencyPerResource
			from
				PurchaseOrders po
				left join CurrencyTypes ct
					on po.CurrencyTypeId = ct.Id
				left join CurrencyExchangeRates cer
					on cer.DestinationCurrencyId = po.CurrencyTypeId and cer.SourceCurrencyId = @workingCurrency
			where
				po.ResourceTypeId = @workingResource
				and
				(
					po.CurrencyTypeId = @workingCurrency
					or
					cer.SourceMultiplier is not null
				)
			) sub
		order by
			ConvertedCurrencyPerResource desc
RETURN 0
