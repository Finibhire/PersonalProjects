CREATE PROCEDURE [dbo].[RefundDeleteRemainingSellOrder]
	@SellOrderId int,
	@UserId int = null
AS
	if (@UserId is null)
	begin
		if not exists(select Id from SellOrders where Id = @SellOrderId)
			throw 51001, '@SellOrderId does not exist', 1
	end
	else
	begin
		if not exists(select Id from SellOrders where Id = @SellOrderId and UserId = @UserId)
			throw 51002, '@SellOrderId does not exist or does not belong to @UserId', 1
	end

	update ur
	set
		OnHand = OnHand + so.ResourceSellAmount - so.ResourceFilledAmount
	from
		UserResources ur
		inner join SellOrders so
			on ur.UserId = so.UserId and ur.ResourceTypeId = so.ResourceTypeId
		inner join ResourceTypes rt
			on so.ResourceTypeId = rt.Id
	where
		so.Id = @SellOrderId

	delete from SellOrders where Id = @SellOrderId
RETURN 0
