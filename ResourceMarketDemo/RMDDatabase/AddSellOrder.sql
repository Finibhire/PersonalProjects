CREATE PROCEDURE [dbo].[AddSellOrder]
	@UserId int,
	@ResourceTypeId int,
	@ResourceSellAmount int,
	@CurrencyTypeId tinyint,
	@CurrencyPerResource float(53)
AS
	--Make sure the client has at least a record for the resource that he is selling
	if not exists(select UserId from UserResources where UserId = @UserId and ResourceTypeId = @ResourceTypeId and OnHand >= @ResourceSellAmount)
		throw 52000, 'UserResources.ResourceTypeId does not exist or client does not have enough OnHand', 1

	--Add a record for the Currency of the SellOrder if it doesn't already exist
	merge UserCurrencies as t
	using (select @UserId, @CurrencyTypeId) as s (UserId, CurrencyTypeId)
	on (t.UserId = s.UserId and t.CurrencyTypeId = s.CurrencyTypeId)
	when not matched then
		insert (UserId, CurrencyTypeId, OnHand)
		values (@UserId, @CurrencyTypeId, 0);

	create table #ConvertedOrders 
	(
		OrderId int not null,
		ConvertedCurrencyPerResource float(53) not null,
		--SigFigCCPR float(53) not null,
		ResourceAmountLeftToFill int not null,
		ResourceAmountToFill int not null,
		RunningTotalResourceAmount int not null,
	)
	
	--Find all PurchaseOrders that are selling the Resource for more than the requested CurrencyPerResource price
	insert into #ConvertedOrders
		(OrderId, ConvertedCurrencyPerResource, ResourceAmountLeftToFill, ResourceAmountToFill, RunningTotalResourceAmount)
	select 
		sub.OrderId,
		sub.ConvertedCurrencyPerResource,
		--dbo.g_OrderSigFigs(ConvertedCurrencyPerResource) as SigFigCCPR,
		sub.ResourceAmountLeftToFill,
		cast(0 as int) as ResourceAmountToFill,
		cast(0 as int) as RunningTotalResourceAmount
	from
		(
			select 
				po.Id as OrderId,
				po.ResourceRequestAmount - po.ResourceFilledAmount as ResourceAmountLeftToFill,
				po.CurrencyPerResource * isnull(cer.SourceMultiplier, 1) as ConvertedCurrencyPerResource
			from
				PurchaseOrders po
				left join CurrencyExchangeRates cer
					on cer.DestinationCurrencyId = @CurrencyTypeId and cer.SourceCurrencyId = po.CurrencyTypeId
			where
				po.UserId != @UserId
				and
				po.ResourceTypeId = @ResourceTypeId
				and
				(
					po.CurrencyTypeId = @CurrencyTypeId
					or
					cer.SourceMultiplier is not null
				)
			) sub
	where
		sub.ConvertedCurrencyPerResource >= @CurrencyPerResource

	--select * from #ConvertedOrders  --debug

	--Update to include running totals
	update co
	set	co.RunningTotalResourceAmount = co2.RunningTotalResourceAmount,
		co.ResourceAmountToFill =
			case
				when co2.RunningTotalResourceAmount <= @ResourceSellAmount
					then co.ResourceAmountLeftToFill
				else @ResourceSellAmount - (co2.RunningTotalResourceAmount - co.ResourceAmountLeftToFill)
			end
	from #ConvertedOrders co
		inner join (
			select 
				co3.OrderId,
				sum(co3.ResourceAmountLeftToFill) over (
						order by co3.ConvertedCurrencyPerResource desc, co3.OrderId 
						rows between unbounded preceding and current row
					) as RunningTotalResourceAmount
			from #ConvertedOrders co3
		) as co2 on co.OrderId = co2.OrderId

	--select * from #ConvertedOrders  --debug

	--Remove all PurchaseOrders that wouldn't be completely filled or partially filled by this SellOrder
	delete co
	from #ConvertedOrders co
	where co.ResourceAmountToFill <= 0 

	begin tran

		--insert any possible missing resources into UserResources for Users that will have part/all of their
		--purchase order filled
		insert into UserResources
		(
			UserId,
			ResourceTypeId,
			OnHand
		)
		select 
			po.UserId,
			@ResourceTypeId as ResourceTypeId,
			cast(0 as bigint) as OnHand
		from
			#ConvertedOrders co
			inner join PurchaseOrders po on co.OrderId = po.Id
			left join UserResources ur on po.ResourceTypeId = @ResourceTypeId and ur.UserId = po.UserId
		where
			ur.OnHand is null
		group by
			po.UserId
		
		--select * from #ConvertedOrders  --debug
		
		declare @ResourceFilledAmount int
		declare @newOnHand decimal(38,9)
		declare @runningCost decimal(38,9)

		set	@ResourceFilledAmount = (select sum(ResourceAmountToFill) from #ConvertedOrders)

		--check to see if we have any PurchaseOrders to fill, if we don't make sure the variables are set to 0
		--and short-circuit past all the statements to update the database when filling PurchaseOrders
		if @ResourceFilledAmount is null or @ResourceFilledAmount = 0
		begin
			set @ResourceFilledAmount = 0
			set @runningCost = cast(0 as decimal(38,9))
		end
		else
		begin
			--template all the MarketSales that we will create in a temporary table so we can use those templated
			--trasactions to create correlating updates in the tables UserCurrencies, UserResources, SellOrders 
			--and PurchaseOrders
			create table #newMarketSales
			(
				SellerUserId int not null,
				BuyerUserId int not null,
				--ResourceTypeId int,
				ResourcesSoldAmount int not null,
				CurrencyTypeId tinyint not null,
				TotalCurrencyCost decimal(38,9) not null
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
				@UserId as SellerUserId,
				po.UserId as BuyerUserId,
				--@ResourceTypeId as ResourceTypeId,
				co.ResourceAmountToFill as ResourcesSoldAmount,
				po.CurrencyTypeId,
				dbo.fRoundDecimalUp(cast(cast(co.ResourceAmountToFill as float(53)) * po.CurrencyPerResource as decimal(38,9)), ct.MaxScale) as TotalCurrencyCost
			from #ConvertedOrders co
				inner join PurchaseOrders po on co.OrderId = po.Id
				inner join CurrencyTypes ct on po.CurrencyTypeId = ct.Id

			--select * from #newMarketSales  --debug

			create table #newCurrencyExchanges
			(
				UserId int not null,
				SourceCurrencyTypeId tinyint not null,
				--DestinationCurrencyTypeId tinyint,
				SourceAmount decimal(38,9) not null,
				DestinationAmount decimal(38,9) not null
			)

			declare @currencyMaxScale int
			select @currencyMaxScale = MaxScale from CurrencyTypes where Id = @CurrencyTypeId

			insert into #newCurrencyExchanges
			(
				UserId,
				SourceCurrencyTypeId,
				SourceAmount,
				DestinationAmount
			)
			select 
				nms.BuyerUserId as UserId,
				nms.CurrencyTypeId as SourceCurrencyTypeId,
				--@CurrencyTypeId as DestinationCurrencyTypeId,
				nms.TotalCurrencyCost as SourceAmount,
				dbo.fRoundDecimalDown(cast(cast(nms.TotalCurrencyCost as float(53)) * cer.SourceMultiplier as decimal(38,9)), @currencyMaxScale) as DestinationAmount
			from
				#newMarketSales nms
				inner join CurrencyExchangeRates cer on cer.SourceCurrencyId = nms.CurrencyTypeId and cer.DestinationCurrencyId = @CurrencyTypeId

			--select * from #newCurrencyExchanges --debug

			--update the database to reflect all the changes processed so far.
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
			
			select * from UserCurrencies where UserId = @UserId --debug

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
				@CurrencyTypeId as DestinationCurrencyTypeId,
				SourceAmount,
				DestinationAmount
			from #newCurrencyExchanges

			--select * from UserCurrencies where UserId = @UserId --debug

			--remove the resources required to fill the PurchaseOrders
			update ur
			set
				ur.OnHand = ur.OnHand - @ResourceFilledAmount
			from
				UserResources ur
			where
				ur.UserId = @UserId
				and
				ur.ResourceTypeId = @ResourceTypeId
				
			--select * from UserCurrencies where UserId = @UserId --debug

			--Remove PurchaseOrders that we completely filled from the database
			delete po 
			from 
				PurchaseOrders po
				inner join #ConvertedOrders co on po.Id = co.OrderId
			where
				co.ResourceAmountLeftToFill = co.ResourceAmountToFill

			--Update the ResourceFilledAmount of any PurchaseOrders we didn't completely fill
			update po
			set
				po.ResourceFilledAmount = po.ResourceFilledAmount + co.ResourceAmountToFill
			from
				PurchaseOrders po
				inner join #ConvertedOrders co on po.Id = co.OrderId
			where
				co.ResourceAmountLeftToFill > co.ResourceAmountToFill

		end --Filling any PurchaseOrders in the db that have equal or better CurrencyPerResource

		if @ResourceFilledAmount < @ResourceSellAmount
		begin
			--resources are not all sold yet and there are no more purchase orders that are greater than or equal
			--to this sell order PricePerResource
			insert into SellOrders
			(
				UserId,
				CurrencyTypeId,
				CurrencyPerResource,
				ResourceTypeId,
				ResourceSellAmount,
				ResourceFilledAmount
			)
			values
			(
				@UserId,
				@CurrencyTypeId,
				@CurrencyPerResource,
				@ResourceTypeId,
				@ResourceSellAmount,
				@ResourceFilledAmount
			)
		end
		
		--//Client definately does have enough resources on hand to make these transactions.  It was calculated
		--//at the begining and isn't variable like the currency exchanged which is why we need to check at the
		--//end of AddPurchaseOrder to see if the client hase enough currency to purcahse all the resources.
	commit tran
RETURN 0
