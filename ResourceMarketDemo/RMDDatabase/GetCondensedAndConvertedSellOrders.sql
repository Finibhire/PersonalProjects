CREATE PROCEDURE [dbo].[GetCondensedAndConvertedSellOrders]
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
				isnull(sum(so.ResourceSellAmount - so.ResourceFilledAmount), 0) as FillableResourceAmount,
				--po.CurrencyTypeId as OriginalCurrencyTypeId,
				ct.Name as OriginalCurrencyName,
				isnull(cer.SourceMultiplier, 1) as SourceMultiplier,
				isnull(
					dbo.g_OrderSigFigsFloor(so.CurrencyPerResource * isnull(cer.SourceMultiplier, 1)),
					0
				) as ConvertedCurrencyPerResource
			from
				SellOrders so
				left join CurrencyTypes ct
					on so.CurrencyTypeId = ct.Id
				left join CurrencyExchangeRates cer
					on cer.DestinationCurrencyId = @workingCurrency and cer.SourceCurrencyId = so.CurrencyTypeId
			where
				so.ResourceTypeId = @workingResource
				and
				(
					so.CurrencyTypeId = @workingCurrency
					or
					cer.SourceMultiplier is not null
				)
			group by
				so.CurrencyPerResource, ct.Name, cer.SourceMultiplier
			) sub
		order by
			ConvertedCurrencyPerResource desc
RETURN 0
