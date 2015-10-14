CREATE VIEW [dbo].[UserCurrenciesPivoted] AS 

	
	SELECT u.*, p.[1] as CurrencyId_1, p.[2] as CurrencyId_2, p.[3] as CurrencyId_3, p.[4] as CurrencyId_4
	FROM (
		select u.Id as UserId, ct.Id as CurrencyId, uc.OnHand
		from [Users] u
		CROSS JOIN [CurrencyTypes] ct
		LEFT JOIN [UserCurrencies] uc ON u.Id = uc.UserId AND ct.Id = uc.CurrencyTypeId
	) as t
	PIVOT
	(
		SUM([OnHand])
		FOR t.[CurrencyId] IN ([1], [2], [3], [4])
	) as p
	JOIN [Users] u ON p.UserId = p.UserId