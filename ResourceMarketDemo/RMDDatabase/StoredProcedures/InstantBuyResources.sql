CREATE PROCEDURE [dbo].[InstantBuyResources]
	@UserId int,
	@ResourceTypeId int,
	@CurrencyTypeId tinyint,
	@MaxBuyAmount int
AS
	if not exists(select Id from Users where Id = @UserId)
		throw 51003, 'UserId does not exist', 1
	if not exists(select Id from ResourceTypes where Id = @ResourceTypeId)
		throw 51004, 'ResourceTypeId does not exist', 1
	if not exists(select Id from CurrencyTypes where Id = @CurrencyTypeId)
		throw 51005, 'CurrencyTypeId does not exist', 1
	if @MaxBuyAmount is null or @MaxBuyAmount <= 0
		throw 51006, 'Invalid @MaxBuyAmount.  It must be > 0', 1

	declare @TotalResourcesAvailable int

	set @TotalResourcesAvailable = (
			select 
				sum(so.ResourceSellAmount - so.ResourceFilledAmount) 
			from SellOrders so
				left join CurrencyExchangeRates cer
					on cer.DestinationCurrencyId = so.CurrencyTypeId and cer.SourceCurrencyId = @CurrencyTypeId
			where 
				so.ResourceTypeId = @ResourceTypeId 
				and 
				so.UserId <> @UserId
				and
				(
					so.CurrencyTypeId = @CurrencyTypeId
					or
					cer.SourceCurrencyId is not null
				)
		)

	if (@TotalResourcesAvailable is null or @TotalResourcesAvailable <= 0)
		RETURN 0
	else if (@TotalResourcesAvailable < @MaxBuyAmount)
		set @MaxBuyAmount = @TotalResourcesAvailable

	declare @CurrencyPerResource float(53) = cast(99999999999999999999999999999.999999999 as float(53))  --max decimal(38,9)

	exec dbo.AddPurchaseOrder @UserId, @ResourceTypeId, @MaxBuyAmount, @CurrencyTypeId, @CurrencyPerResource

RETURN 0
