CREATE PROCEDURE [dbo].[GetConvertedPurchaseOrders]
	@workingCurrency tinyint,
	@workingResource int
AS
	select 
		Id,
		UserId,
		ResourceTypeId,
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
				po.ResourceTypeId,
				po.ResourceRequestAmount - ResourceFilledAmount as ToBeFilledAmount,
				po.CurrencyTypeId as OriginalCurrencyTypeId,
				ct.Name as OriginalCurrencyName,
				po.CurrencyPerResource,
				isnull(cer.SourceMultiplier, 1) as SourceMultiplier,
				cast(po.CurrencyPerResource as float(53)) / isnull(cast(cer.SourceMultiplier as float(53)), 1) as ConvertedCurrencyPerResource
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
