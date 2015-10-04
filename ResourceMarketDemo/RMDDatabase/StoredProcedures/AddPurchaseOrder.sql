CREATE PROCEDURE [dbo].[AddPurchaseOrder]
	@UserId int,
	@ResourceTypeId int,
	@ResourceRequestAmount int,
	@CurrencyTypeId tinyint,
	@CurrencyPerResource float(53)
AS
	if not exists(select * from UserCurrencies where UserId = @UserId and CurrencyTypeId = @CurrencyTypeId)
	begin
		if (@CurrencyPerResource = cast(0 as float(53)))  --Not sure if I'm going to allow buying for 0 price but if I do then I need to add the record
		begin
			merge UserCurrencies as t
			using (select @UserId, @CurrencyTypeId) as s (UserId, CurrencyTypeId)
			on (t.UserId = s.UserId and t.CurrencyTypeId = s.CurrencyTypeId)
			when not matched then
				insert (UserId, CurrencyTypeId, OnHand)
				values (@UserId, @CurrencyTypeId, 0);
		end
		else
			throw 52000, 'UserCurrency.CurrencyTypeId does not exist', 1
	end

	merge UserResources as t
	using (select @UserId, @ResourceTypeId) as s (UserId, ResourceTypeId)
	on (t.UserId = s.UserId and t.ResourceTypeId = s.ResourceTypeId)
	when not matched then
		insert (UserId, ResourceTypeId, OnHand)
		values (@UserId, @ResourceTypeId, 0);

	create table #ConvertedOrders 
	(
		OrderId int not null,
		ConvertedCurrencyPerResource float(53) not null,
		ResourceAmountLeftToFill int not null,
		ResourceAmountToFill int not null,
		RunningTotalResourceAmount int not null,
	)

	--Find all PurchaseOrders that are selling the Resource for more than the requested CurrencyPerResource price
	insert into #ConvertedOrders
		(OrderId, ConvertedCurrencyPerResource, ResourceAmountLeftToFill, ResourceAmountToFill, RunningTotalResourceAmount)
	select 
		OrderId,
		ConvertedCurrencyPerResource,
		ResourceAmountLeftToFill,
		cast(0 as int) as ResourceAmountToFill,
		cast(0 as int) as RunningTotalResourceAmount
	from
		(
			select 
				so.Id as OrderId,
				so.ResourceSellAmount - ResourceFilledAmount as ResourceAmountLeftToFill,
				so.CurrencyPerResource / isnull(cer.SourceMultiplier, 1) as ConvertedCurrencyPerResource
			from
				SellOrders so
				left join CurrencyExchangeRates cer
					on cer.DestinationCurrencyId = so.CurrencyTypeId and cer.SourceCurrencyId = @CurrencyTypeId
			where
				so.UserId != @UserId
				and
				so.ResourceTypeId = @ResourceTypeId
				and
				(
					so.CurrencyTypeId = @CurrencyTypeId
					or --same currency or conversion exists in the cer table
					cer.SourceMultiplier is not null
				)
			) sub
	where
		sub.ConvertedCurrencyPerResource <= @CurrencyPerResource
		
	--select * from #ConvertedOrders  --debug

	--Update to include running totals
	update co
	set	co.RunningTotalResourceAmount = co2.RunningTotalResourceAmount,
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
						order by co3.ConvertedCurrencyPerResource, co3.OrderId 
						rows between unbounded preceding and current row
					) as RunningTotalResourceAmount
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

		declare @ResourceFilledAmount int
		declare @newOnHand decimal(38,9)
		declare @runningCost decimal(38,9)

		set	@ResourceFilledAmount = (select sum(ResourceAmountToFill) from #ConvertedOrders)

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
				--ResourceTypeId int,
				ResourcesSoldAmount int,
				CurrencyTypeId tinyint,
				TotalCurrencyCost decimal(38,9)
			)
		
			insert into #newMarketSales
			(
				SellerUserId,
				BuyerUserId,
				ResourcesSoldAmount,
				CurrencyTypeId,
				TotalCurrencyCost
			)
			select 
				so.UserId as SellerUserId,
				@UserId as BuyerUserId,
				--@ResourceTypeId as ResourceTypeId,
				co.ResourceAmountToFill as ResourcesSoldAmount,
				so.CurrencyTypeId,
				dbo.fRoundDecimalUp(cast(cast(co.ResourceAmountToFill as float(53)) * so.CurrencyPerResource as decimal(38,9)), ct.MaxScale) as TotalCurrencyCost
			from #ConvertedOrders co
				inner join SellOrders so on co.OrderId = so.Id
				inner join CurrencyTypes ct on so.CurrencyTypeId = ct.Id

			--select * from #newMarketSales  --debug

			create table #newCurrencyExchanges
			(
				UserId int,
				--SourceCurrencyTypeId tinyint,
				DestinationCurrencyTypeId tinyint,
				SourceAmount decimal(38,9),
				DestinationAmount decimal(38,9)
			)

			insert into #newCurrencyExchanges
			(
				UserId,
				DestinationCurrencyTypeId,
				SourceAmount,
				DestinationAmount
			)
			select 
				nms.BuyerUserId as UserId,
				--@CurrencyTypeId as SourceCurrencyTypeId,
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
				@CurrencyTypeId as SourceCurrencyTypeId,
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
				@ResourceTypeId as ResourceTypeId,
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
