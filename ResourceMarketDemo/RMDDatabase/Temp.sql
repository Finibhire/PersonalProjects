insert into CurrencyExchanges
(
	UserId,
	SourceCurrencyTypeId,
	DestinationCurrencyTypeId,
	SourceAmount,
	DestinationAmount
)
select 1, 1, 2, 1, 10
union ALL
select 1, 1, 3, 1, 10
union ALL
select 1, 4, 2, 1, 10
union ALL
select 1, 4, 3, 1, 10
union ALL
select 1, 2, 3, 1, 10



insert into SellOrders
(
	UserId,
	CurrencyTypeId,
	CurrencyPerResource,
	ResourceTypeId,
	ResourceSellAmount,
	ResourceFilledAmount
)
-- gold=1, dp=2, hyper=3
select 2, 1, 10, 1, 100, 0
union all
select 2, 2, 0.1, 1, 100, 0
union all
select 2, 3, 0.00001, 1, 100, 0

select * from SellOrders

exec AddPurchaseOrder 1, 1, 150, 2, 10

select so.*, (cer.SourceMultiplier * so.CurrencyPerResource) as RealCurPerRes 
from SellOrders so left join CurrencyExchangeRates cer on cer.SourceCurrencyId = so.CurrencyTypeId and cer.DestinationCurrencyId = 2
select * from MarketSales



declare @CurrencyTypeId tinyint = 2
declare @ResourceTypeId tinyint = 1
declare @UserId tinyint = 1
declare @CurrencyPerResource decimal(38,9) = 10
	select 
		so.Id as OrderId, 
		case 
			when cer.SourceMultiplier is null
				then so.CurrencyPerResource
			else
				so.CurrencyPerResource / cer.SourceMultiplier
		end as CurrencyPerResource,
		so.ResourceSellAmount - so.ResourceFilledAmount as ResourceAmount, 
		cast(0 as int) as RunningTotalResourceAmount,
		case 
			when cer.SourceMultiplier is null
				then cast(so.ResourceSellAmount - so.ResourceFilledAmount as decimal(38,9)) * so.CurrencyPerResource
			else
				cast(so.ResourceSellAmount - so.ResourceFilledAmount as decimal(38,9)) * so.CurrencyPerResource / cer.SourceMultiplier
		end as TotalCost,
		cast(0 as decimal(38,9)) as RunningTotalCost
	from SellOrders so
	full outer join CurrencyExchangeRates cer 
		on cer.SourceCurrencyId = @CurrencyTypeId and cer.DestinationCurrencyId = so.CurrencyTypeId
	where
		so.ResourceTypeId = @ResourceTypeId
		and
		so.UserId != @UserId  --don't include sellorders that are created by this user
		and
		(
			(
				so.CurrencyTypeId = @CurrencyTypeId 
				and
				so.CurrencyPerResource <= @CurrencyPerResource  --don't include orders that are more expensive
			)
			or
			(
				cer.DestinationCurrencyId is not null
				and
				so.CurrencyPerResource / cer.SourceMultiplier <= @CurrencyPerResource  --dido
			)
		)