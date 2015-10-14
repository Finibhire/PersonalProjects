CREATE PROCEDURE [dbo].[InstantSellResources]
	@UserId int,
	@ResourceTypeId int,
	@CurrencyTypeId tinyint,
	@MaxSellAmount int
AS
	if not exists(select Id from Users where Id = @UserId)
		throw 51003, 'UserId does not exist', 1
	if not exists(select Id from ResourceTypes where Id = @ResourceTypeId)
		throw 51004, 'ResourceTypeId does not exist', 1
	if not exists(select Id from CurrencyTypes where Id = @CurrencyTypeId)
		throw 51005, 'CurrencyTypeId does not exist', 1
	if @MaxSellAmount is null or @MaxSellAmount <= 0
		throw 51006, 'Invalid @MaxSellAmount.  It must be > 0', 1
	--the check to see if the user has enough on hand is done in AddSellOrder stored procedure


	declare @TotalResourcesAvailable int

	set @TotalResourcesAvailable = (
			select 
				sum(po.ResourceRequestAmount - po.ResourceFilledAmount) 
			from 
				PurchaseOrders po
				left join CurrencyExchangeRates cer
					on cer.SourceCurrencyId = po.CurrencyTypeId and cer.DestinationCurrencyId = @CurrencyTypeId
			where 
				po.ResourceTypeId = @ResourceTypeId 
				and 
				po.UserId <> @UserId
				and
				(
					po.CurrencyTypeId = @CurrencyTypeId
					or
					cer.SourceCurrencyId is not null
				)
		)

	if (@TotalResourcesAvailable is null or @TotalResourcesAvailable <= 0)
		RETURN 0
	else if (@TotalResourcesAvailable < @MaxSellAmount)
		set @MaxSellAmount = @TotalResourcesAvailable

	declare @CurrencyPerResource float(53) = cast(0.000000001 as float(53))  --smallest non-zero positive decimal(38,9)

	exec dbo.AddSellOrder @UserId, @ResourceTypeId, @MaxSellAmount, @CurrencyTypeId, @CurrencyPerResource

RETURN 0
