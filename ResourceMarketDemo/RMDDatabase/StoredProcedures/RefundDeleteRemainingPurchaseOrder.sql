CREATE PROCEDURE [dbo].[RefundDeleteRemainingPurchaseOrder]
	@PurchaseOrderId int,
	@UserId int = null
AS
	if (@UserId is null)
	begin
		if not exists(select Id from PurchaseOrders where Id = @PurchaseOrderId)
			throw 51001, '@PurchaseOrderId does not exist', 1
	end
	else
	begin
		if not exists(select Id from PurchaseOrders where Id = @PurchaseOrderId and UserId = @UserId)
			throw 51002, '@PurchaseOrderId does not exist or does not belong to @UserId', 1
	end

	update uc
	set
		OnHand = OnHand + dbo.fRoundDecimalDown(cast(cast(po.ResourceRequestAmount - po.ResourceFilledAmount as float(53)) * po.CurrencyPerResource as decimal(38,9)), ct.MaxScale)
	from
		UserCurrencies uc
		inner join PurchaseOrders po
			on uc.UserId = po.UserId and uc.CurrencyTypeId = po.CurrencyTypeId
		inner join CurrencyTypes ct
			on po.CurrencyTypeId = ct.Id
	where
		po.Id = @PurchaseOrderId

	delete from PurchaseOrders where Id = @PurchaseOrderId
RETURN 0
