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

begin tran
select * from SellOrders
exec AddPurchaseOrder 1, 1, 150, 2, 10
select * from UserCurrencies
select so.*, (cer.SourceMultiplier / so.CurrencyPerResource) as RealCurPerRes 
from SellOrders so left join CurrencyExchangeRates cer on cer.SourceCurrencyId = so.CurrencyTypeId and cer.DestinationCurrencyId = 2
select * from MarketSales

rollback tran


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


merge UserCurrencies as t
using 
(
	select 
		cast(1 as int) as UserId, 
		cast(1 as tinyint) as CurrencyTypeId, 
		cast(5 as decimal(38,9)) as OnHand
) as s
on (t.UserId = s.UserId and t.CurrencyTypeId = s.CurrencyTypeId)
when matched then
	update set t.OnHand = s.OnHand
when not matched then
	insert (UserId, CurrencyTypeId, OnHand)
	values (s.UserId, s.CurrencyTypeId, s.OnHand)
output deleted.*, inserted.*;

select * from UserCurrencies




declare @maxVal decimal(38,9) = 99999999999999999999999999999.999999999

select 
	@maxVal as [all],
	floor(@maxVal) as whole,
	cast(@maxVal - cast(floor(@maxVal) as decimal(29,0)) as decimal(9,9)) as fraction,
	cast(cast(@maxVal - cast(floor(@maxVal) as decimal(29,0)) as decimal(9,9)) * cast(1000000000 as decimal(10,0)) as decimal(9,0)) as wholeFraction





begin tran
select * from UserResources where ResourceTypeId = 1 and UserId <= 2
select * from UserCurrencies where (CurrencyTypeId = 1 or CurrencyTypeId = 2) and UserId <= 2
exec dbo.AddSellOrder 2, 1, 10, 1, 0.5
select * from UserResources where ResourceTypeId = 1 and UserId <= 2
select * from UserCurrencies where (CurrencyTypeId = 1 or CurrencyTypeId = 2) and UserId <= 2
rollback tran

