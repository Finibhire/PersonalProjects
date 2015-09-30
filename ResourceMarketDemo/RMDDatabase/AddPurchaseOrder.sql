CREATE PROCEDURE [dbo].[AddPurchaseOrder]
	@UserId int,
	@ResourceTypeId int,
	@ResourceRequestAmount int,
	@CurrencyTypeId tinyint,
	@CurrencyPerResource float(53)
AS
	declare @newOnHand decimal(38,9)
	declare @ResourceFilledAmount int

	--set @newOnHand = 0
	--set @ResourceFilledAmount = 0

	if not exists(select * from UserCurrencies where UserId = @UserId and CurrencyTypeId = @CurrencyTypeId)
		throw 52000, 'UserCurrency.CurrencyTypeId does not exist', 1

	if not exists(select * from UserResources where UserId = @UserId and ResourceTypeId = @ResourceTypeId)
	begin
		insert into UserResources
		(
			UserId,
			ResourceTypeId,
			OnHand
		)
		values
		(
			@UserId,
			@ResourceTypeId,
			cast(0 as bigint)
		)
	end

	create table #ConvertedOrders 
	(
		OrderId int,
		--UserId int,
		--ResourceTypeId int,
		CurrencyPerResource decimal(38,9),
		ResourceAmountLeftToFill int,
		ResourceAmountToFill int,
		RunningTotalResourceAmount int,
		--CurrencyTypeId tinyint,
		--TotalCost decimal(38,9),
		--RunningTotalCost decimal(38,9)
	)

	insert into #ConvertedOrders
	select 
		so.Id as OrderId, 
		case 
			when cer.SourceMultiplier is null
				then so.CurrencyPerResource
			else
				so.CurrencyPerResource / cer.SourceMultiplier
		end as CurrencyPerResource,
		so.ResourceSellAmount - so.ResourceFilledAmount as ResourceAmountLeftToFill, 
		cast(0 as int) as ResourceAmountToFill,
		cast(0 as int) as RunningTotalResourceAmount--,
		--case 
		--	when cer.SourceMultiplier is null
		--		then dbo.fRoundDecimalUp(cast(so.ResourceSellAmount - so.ResourceFilledAmount as decimal(38,9)) * so.CurrencyPerResource, so_ct.MaxScale)
		--	else
		--		dbo.fRoundDecimalUp(dbo.fRoundDecimalUp(cast(so.ResourceSellAmount - so.ResourceFilledAmount as decimal(38,9)) * so.CurrencyPerResource, cer_ct.MaxScale) / cer.SourceMultiplier, so_ct.MaxScale)
		--end as TotalCost,
		--cast(0 as decimal(38,9)) as RunningTotalCost
	from SellOrders so
	full outer join CurrencyExchangeRates cer 
		on cer.SourceCurrencyId = @CurrencyTypeId and cer.DestinationCurrencyId = so.CurrencyTypeId
	left join CurrencyTypes so_ct
		on so.CurrencyTypeId = so_ct.Id
	left join CurrencyTypes cer_ct
		on cer.DestinationCurrencyId = cer_ct.Id
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

	--Update to include running totals
	update co
	set	co.RunningTotalResourceAmount = co2.RunningTotalResourceAmount,
		--co.RunningTotalCost = co2.RunningTotalCost,
		co.ResourceAmountToFill =
			case
				when co2.RunningTotalResourceAmount < @ResourceRequestAmount
					then co.ResourceAmountLeftToFill
				else @ResourceRequestAmount - (co2.RunningTotalResourceAmount - co.ResourceAmountLeftToFill)
			end
	from #ConvertedOrders co
		inner join (
			select 
				co3.OrderId,
				sum(co3.ResourceAmountLeftToFill) over (
						order by co3.CurrencyPerResource, co3.OrderId 
						rows between unbounded preceding and current row
					) as RunningTotalResourceAmount--,
				--sum(co3.TotalCost) over (
				--		order by co3.CurrencyPerResource, co3.OrderId 
				--		rows between unbounded preceding and current row
				--	) as RunningTotalCost
			from #ConvertedOrders co3
		) as co2 on co.OrderId = co2.OrderId

	--select * from #ConvertedOrders  --debug

	begin tran

		--insert any possible missing currencies into UserCurrencies for @UserId that might be needed
		--index(@UserId, @CurrencyTypeId) as already been checked to exist at the start of stored procedure.
		insert into UserCurrencies
		(
			UserId,
			CurrencyTypeId,
			OnHand
		)
		select 
			@UserId as UserId,
			so.CurrencyTypeId,
			cast(0 as decimal(38,9)) as OnHand
		from
			#ConvertedOrders co
			inner join SellOrders so on co.OrderId = so.Id
			left join UserCurrencies uc on so.CurrencyTypeId = uc.CurrencyTypeId and uc.UserId = @UserId
		where
			uc.OnHand is null
		group by
			so.CurrencyTypeId

		--Remove all SellOrders that wouldn't be completely filled or partially filled by this PurchaseOrder
		delete co
		from #ConvertedOrders co
		where co.ResourceAmountToFill <= 0 
			--or CurrencyPerResource <= @CurrencyPerResource
		
		--select * from #ConvertedOrders  --debug

		declare @runningCost decimal(38,9)

		select 
			@ResourceFilledAmount = sum(ResourceAmountToFill)
		from #ConvertedOrders

		--check to see if we have any full SellOrders to fill, if we don't make sure the variables are set to 0
		--and short-circuit past all the statements to update the database when filling SellOrders in the db already.
		if @ResourceFilledAmount is null or @ResourceFilledAmount = 0
		begin
			set @ResourceFilledAmount = 0
			set @runningCost = 0
		end
		else
		begin
			--template all the MarketSales that we will create in a temporary table so we can use those templated
			--trasactions to create correlating updates in CurrencyExchanges, UserCurrencies and SellOrders tables
			create table #newMarketSales
			(
				SellerUserId int,
				BuyerUserId int,
				ResourceTypeId int,
				ResourcesSoldAmount int,
				CurrencyTypeId tinyint,
				TotalCurrencyCost decimal(38,9)
			)
		
			insert into #newMarketSales
			select 
				so.UserId as SellerUserId,
				@UserId as BuyerUserId,
				@ResourceTypeId as ResourceTypeId,
				co.ResourceAmountToFill as ResourcesSoldAmount,
				so.CurrencyTypeId,
				dbo.fRoundDecimalUp(cast(co.ResourceAmountToFill as decimal(38,9)) * so.CurrencyPerResource, ct.MaxScale) as TotalCurrencyCost
			from #ConvertedOrders co
				inner join SellOrders so on co.OrderId = so.Id
				inner join CurrencyTypes ct on so.CurrencyTypeId = ct.Id

			--select * from #newMarketSales  --debug

			create table #newCurrencyExchanges
			(
				UserId int,
				SourceCurrencyTypeId tinyint,
				DestinationCurrencyTypeId tinyint,
				SourceAmount decimal(38,9),
				DestinationAmount decimal(38,9)
			)

			insert into #newCurrencyExchanges
			select 
				nms.BuyerUserId as UserId,
				@CurrencyTypeId as SourceCurrencyTypeId,
				nms.CurrencyTypeId as DestinationCurrencyTypeId,
				dbo.fRoundDecimalUp(nms.TotalCurrencyCost / cer.SourceMultiplier, ct.MaxScale) as SourceAmount,
				nms.TotalCurrencyCost as DestinationAmount
			from
				#newMarketSales nms
				inner join CurrencyExchangeRates cer on cer.SourceCurrencyId = @CurrencyTypeId and cer.DestinationCurrencyId = nms.CurrencyTypeId
				inner join CurrencyTypes ct on cer.DestinationCurrencyId = ct.Id

			--select * from #newCurrencyExchanges --debug

			--update the database to reflect all the changes processed so far.
			insert into CurrencyExchanges
			(
				UserId,
				SourceCurrencyTypeId,
				DestinationCurrencyTypeId,
				SourceAmount,
				DestinationAmount
			)
			select 
				UserId,
				SourceCurrencyTypeId,
				DestinationCurrencyTypeId,
				SourceAmount,
				DestinationAmount
			from #newCurrencyExchanges

			--select * from UserCurrencies where UserId = @UserId --debug

			insert into MarketSales
			(
				SellerUserId,
				BuyerUserId,
				ResourceTypeId,
				ResourcesSoldAmount,
				CurrencyTypeId,
				TotalCurrencyCost
			)
			select 
				SellerUserId,
				BuyerUserId,
				ResourceTypeId,
				ResourcesSoldAmount,
				CurrencyTypeId,
				TotalCurrencyCost
			from #newMarketSales
			
			--select * from UserCurrencies where UserId = @UserId --debug

			--remove the currency amounts that were added to the @UserId account by inserting into CurrencyExchanges
			--because they are consumed by filling SellOrders
			update uc
			set
				uc.OnHand = uc.OnHand - nce.TotalDestinationAmount
			from
				UserCurrencies uc
				inner join (
					select 
						nce2.UserId,
						nce2.DestinationCurrencyTypeId,
						sum(nce2.DestinationAmount) as TotalDestinationAmount
					from
						#newCurrencyExchanges nce2
					group by
						nce2.UserId, nce2.DestinationCurrencyTypeId
				) as nce on uc.UserId = nce.UserId and uc.CurrencyTypeId = nce.DestinationCurrencyTypeId
				
			--select * from UserCurrencies where UserId = @UserId --debug

			--remove the currency required to fill the SellOrders that were in the same currency as the PurchaseOrder
			update uc
			set
				uc.OnHand = uc.OnHand - nms.sumTotalCurrencyCost
			from
				UserCurrencies uc
				inner join (
					select
						nms2.BuyerUserId,
						sum(nms2.TotalCurrencyCost) as sumTotalCurrencyCost
					from
						UserCurrencies uc2
						inner join #newMarketSales nms2 on uc2.UserId = nms2.BuyerUserId
					where
						nms2.CurrencyTypeId = @CurrencyTypeId
					group by
						nms2.BuyerUserId
				) as nms on uc.UserId = nms.BuyerUserId
				
			--select * from UserCurrencies where UserId = @UserId --debug

			--Remove SellOrders that we completely filled from the database
			delete so 
			from 
				SellOrders so
				inner join #ConvertedOrders co on so.Id = co.OrderId
			where
				co.ResourceAmountLeftToFill = co.ResourceAmountToFill

			--Update the ResourceFilledAmount of any SellOrders we didn't completely fill
			update so
			set
				so.ResourceFilledAmount = so.ResourceFilledAmount + co.ResourceAmountToFill
			from
				SellOrders so
				inner join #ConvertedOrders co on so.Id = co.OrderId
			where
				co.ResourceAmountLeftToFill > co.ResourceAmountToFill

		end --Filling any SellOrders in the db that have equal or better CurrencyPerResource

		--truncate table #ConvertedOrders
		--truncate table #newMarketSales
		--truncate table #newCurrencyExchanges

		if @ResourceFilledAmount < @ResourceRequestAmount
		begin
			--resources are not all filled yet and there are no more sell orders that are less than or equal
			--to the purchase order PricePerResource
			insert into PurchaseOrders
			(
				UserId,
				CurrencyTypeId,
				CurrencyPerResource,
				ResourceTypeId,
				ResourceRequestAmount,
				ResourceFilledAmount
			)
			values
			(
				@UserId,
				@CurrencyTypeId,
				@CurrencyPerResource,
				@ResourceTypeId,
				@ResourceRequestAmount,
				@ResourceFilledAmount
			)
		end
		
		set @newOnHand = null
		select @newOnHand = OnHand from UserCurrencies where UserId = @UserId and CurrencyTypeId = @CurrencyTypeId

		if @newOnHand is null or @newOnHand < cast(0 as decimal(38,9))
			throw 51000, 'User does not have enough currency on hand to complete this transaction', 1
	commit tran
RETURN 0
