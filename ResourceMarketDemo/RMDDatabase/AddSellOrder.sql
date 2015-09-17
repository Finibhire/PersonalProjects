CREATE PROCEDURE [dbo].[AddSellOrder]
	@UserId int,
	@ResourceTypeId int,
	@ResourceSellAmount int,
	@CurrencyTypeId tinyint,
	@CurrencyPerResource decimal(38,9)
AS
	declare @newOnHand decimal(38,9)

	select @newOnHand = OnHand - @ResourceSellAmount
	from UserResources
	where UserId = @UserId and ResourceTypeId = @ResourceTypeId

	if @newOnHand is null or @newOnHand < 0
		throw 51000, 'User does not have enough of the resource on hand to create the SellOrder.', 1
	
	begin tran
		insert into PurchaseOrders (UserId, ResourceTypeId, ResourceRequestAmount, CurrencyTypeId, CurrencyPerResource)
		values (@UserId, @ResourceTypeId, @ResourceSellAmount, @CurrencyTypeId, @CurrencyPerResource)

		update ur
		set OnHand = @newOnHand
		from UserResources ur
		where UserId = @UserId and ResourceTypeId = @ResourceTypeId
	commit tran
RETURN 0
