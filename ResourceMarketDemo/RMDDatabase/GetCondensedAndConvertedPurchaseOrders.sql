CREATE PROCEDURE [dbo].[GetCondensedAndConvertedPurchaseOrders]
	@workingCurrency tinyint,
	@workingResource int
AS
	select 
		FillableResourceAmount,
		--OriginalCurrencyTypeId,
		OriginalCurrencyName,
		SourceMultiplier,
		ConvertedCurrencyPerResource
	from
		(
			select 
				isnull(sum(po.ResourceRequestAmount - po.ResourceFilledAmount), 0) as FillableResourceAmount,
				--po.CurrencyTypeId as OriginalCurrencyTypeId,
				ct.Name as OriginalCurrencyName,
				isnull(cer.SourceMultiplier, 1) as SourceMultiplier,
				isnull(
					dbo.g_OrderSigFigsFloor(po.CurrencyPerResource / isnull(cer.SourceMultiplier, 1)),
					0
				) as ConvertedCurrencyPerResource
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
			group by
				po.CurrencyPerResource, ct.Name, cer.SourceMultiplier
			) sub
		order by
			ConvertedCurrencyPerResource desc
RETURN 0
