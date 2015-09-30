CREATE PROCEDURE [dbo].[GetConvertedSellOrders]
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
				so.Id,
				so.UserId,
				so.ResourceSellAmount - so.ResourceFilledAmount as ToBeFilledAmount,
				so.CurrencyTypeId as OriginalCurrencyTypeId,
				ct.Name as OriginalCurrencyName,
				isnull(cer.SourceMultiplier, 1) as SourceMultiplier,
				dbo.g_OrderSigFigsCeiling(so.CurrencyPerResource / isnull(cer.SourceMultiplier, 1)) as ConvertedCurrencyPerResource
			from
				SellOrders so
				left join CurrencyTypes ct
					on so.CurrencyTypeId = ct.Id
				left join CurrencyExchangeRates cer
					on cer.DestinationCurrencyId = so.CurrencyTypeId and cer.SourceCurrencyId = @workingCurrency
			where
				so.ResourceTypeId = @workingResource
				and
				(
					so.CurrencyTypeId = @workingCurrency
					or
					cer.SourceMultiplier is not null
				)
		) sub
	order by
		ConvertedCurrencyPerResource

RETURN 0

