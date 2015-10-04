CREATE PROCEDURE [dbo].[DynamicUpdateUserPivotViews]
AS

DECLARE @resourceCols as nvarchar(max)
DECLARE @resourceCols2 as nvarchar(max)
DECLARE @currencyCols as nvarchar(max)
DECLARE @currencyCols2 as nvarchar(max)
DECLARE @query as nvarchar(max)

DECLARE @resourceRows as bit
DECLARE @currencyRows as bit

select @resourceRows = cast(
		case when EXISTS(select Id from ResourceTypes) then 1
		else 0
		end
	as bit)
select @currencyRows = cast(
		case when EXISTS(select Id from CurrencyTypes) then 1
		else 0
		end
	as bit)

IF @resourceRows = cast(1 as bit)
BEGIN

	set @resourceCols = (
			select ',' + quotename(rt.Name)
			from ResourceTypes rt
			order by rt.Name
			for xml path(''), type
		).value('.', 'nvarchar(max)')

	set @resourceCols2 = REPLACE(@resourceCols, ',', ',pur.')

	set @resourceCols = STUFF(@resourceCols, 1, 1, '')

	IF EXISTS(
			select [TABLE_NAME] 
			from INFORMATION_SCHEMA.VIEWS 
			where [TABLE_NAME] = 'UserResourcesPivoted'
		)
	DROP VIEW [UserResourcesPivoted]

	set @query =
		N'CREATE VIEW [dbo].[UserResourcesPivoted] AS 
			SELECT u1.*' + @resourceCols2 + N'
			FROM (
				select u2.Id as UserId, rt.Name as ResourceName, ur2.OnHand
				from [Users] u2
				CROSS JOIN [ResourceTypes] rt
				LEFT JOIN [UserResources] ur2 ON u2.Id = ur2.UserId AND rt.Id = ur2.ResourceTypeId
			) as ur1
			PIVOT
			(
				SUM([OnHand])
				FOR ur1.[ResourceName] IN (' + @resourceCols + N')
			) as pur
			JOIN [Users] u1 ON u1.Id = pur.UserId'
	
	exec sp_executesql @query;
	--select * from UserResourcesPivoted
END
ELSE
BEGIN
	IF EXISTS(
			select [TABLE_NAME] 
			from INFORMATION_SCHEMA.VIEWS 
			where [TABLE_NAME] = 'UserResourcesPivoted'
		)
	DROP VIEW [UserResourcesPivoted]
END


IF @currencyRows = cast(1 as bit)
BEGIN
	set @currencyCols = (
			select ',' + quotename(ct.Name)
			from CurrencyTypes ct
			order by ct.Name
			for xml path(''), type
		).value('.', 'nvarchar(max)')
	
	set @currencyCols2 = REPLACE(@currencyCols, ',', ',puc.')

	set @currencyCols = STUFF(@currencyCols, 1, 1, '')

	IF EXISTS(
			select [TABLE_NAME] 
			from INFORMATION_SCHEMA.VIEWS 
			where [TABLE_NAME] = 'UserCurrenciesPivoted'
		)
	DROP VIEW [UserCurrenciesPivoted]

	set @query =
		N'CREATE VIEW [dbo].[UserCurrenciesPivoted] AS 
			SELECT u1.*' + @currencyCols2 + N'
			FROM (
				select u2.Id as UserId, ct.Name as CurrencyName, uc2.OnHand
				from [Users] u2
				CROSS JOIN [CurrencyTypes] ct
				LEFT JOIN [UserCurrencies] uc2 ON u2.Id = uc2.UserId AND ct.Id = uc2.CurrencyTypeId
			) as uc1
			PIVOT
			(
				SUM([OnHand])
				FOR uc1.[CurrencyName] IN (' + @currencyCols + N')
			) as puc
			JOIN [Users] u1 ON u1.Id = puc.UserId'
	
	exec sp_executesql @query;
	--select * from UserCurrenciesPivoted
END
ELSE
BEGIN
	IF EXISTS(
			select [TABLE_NAME] 
			from INFORMATION_SCHEMA.VIEWS 
			where [TABLE_NAME] = 'UserCurrenciesPivoted'
		)
	DROP VIEW [UserCurrenciesPivoted]
END


IF @resourceRows = cast(1 as bit) AND @currencyRows = cast(1 as bit)
BEGIN
	IF EXISTS(
			select [TABLE_NAME] 
			from INFORMATION_SCHEMA.VIEWS 
			where [TABLE_NAME] = 'UserAllPivoted'
		)
	DROP VIEW [UserAllPivoted]

	set @query = 
		N'CREATE VIEW [dbo].[UserAllPivoted] AS 
			SELECT u1.*' + @currencyCols2 + @resourceCols2 + N'
			FROM 
				[Users] u1
			LEFT JOIN (
					select u2.Id as UserId, rt.Name as ResourceName, ur2.OnHand
					from [Users] u2
					CROSS JOIN [ResourceTypes] rt
					LEFT JOIN [UserResources] ur2 ON u2.Id = ur2.UserId AND rt.Id = ur2.ResourceTypeId
				) as ur1
				PIVOT (
					SUM([OnHand])
					FOR ur1.[ResourceName] IN (' + @resourceCols + N')
				) as pur
				ON u1.Id = pur.UserId
			LEFT JOIN (
					select u3.Id as UserId, ct.Name as CurrencyName, uc2.OnHand
					from [Users] u3
					CROSS JOIN [CurrencyTypes] ct
					LEFT JOIN [UserCurrencies] uc2 ON u3.Id = uc2.UserId AND ct.Id = uc2.CurrencyTypeId
				) as uc1
				PIVOT
				(
					SUM([OnHand])
					FOR uc1.[CurrencyName] IN (' + @currencyCols + N')
				) as puc 
				ON u1.Id = puc.UserId'


	exec sp_executesql @query;
	--select * from UserAllPivoted
END
ELSE
BEGIN
	IF EXISTS(
			select [TABLE_NAME] 
			from INFORMATION_SCHEMA.VIEWS 
			where [TABLE_NAME] = 'UserAllPivoted'
		)
	DROP VIEW [UserAllPivoted]
END


RETURN 0 --success
