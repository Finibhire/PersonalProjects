/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/


-- Reference Data for CurrencyTypes
MERGE INTO CurrencyTypes AS Target 
USING (VALUES 
  (1, N'Gold', 9), 
  (2, N'Dragon Points', 9), 
  (3, N'HyperCoin', 9),
  (4, N'HTML5', 9),
  (5, N'Gold Points', 9),
  (6, N'FLAP', 9)
) 
AS Source (Id, Name, MaxScale) 
ON Target.Id = Source.Id
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET Name = Source.Name, MaxScale = Source.MaxScale
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (Id, Name, MaxScale) 
VALUES (Id, Name, MaxScale) 
-- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;


-- Reference Data for ResourceTypes
MERGE INTO ResourceTypes AS Target
USING (VALUES
	(1, N'Wood'),
	(2, N'Fish'),
	(3, N'Stone'),
	(4, N'Iron')
)
AS Source (Id, Name)
ON Target.Id = Source.Id
WHEN MATCHED THEN 
UPDATE SET Name = Source.Name
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (Id, Name) 
VALUES (Id, Name) 
-- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;

-- Reference Data for ExchangeRates
declare @DPtoGold decimal(38,9)
declare @DPtoHyperCoin decimal(38,9)
declare @DPtoHTML5 decimal(38,9)
declare @DPtoGoldPoints decimal(38,9)
declare @DPtoFLAP decimal(38,9)
declare @ConvertTaxRate decimal(38,9)

set @DPtoGold		= 10
set @DPtoHyperCoin	= 0.00008894
set @DPtoHTML5		= 8.22637000
set @DPtoGoldPoints = 0.00024041
set @DPtoFLAP		= 6.37544000
set @ConvertTaxRate = 0.85

MERGE INTO CurrencyExchangeRates AS Target 
USING (VALUES 
  (2, 1, @DPtoGold),
  (2, 3, @DPtoHyperCoin * @ConvertTaxRate),
  (2, 4, @DPtoHTML5 * @ConvertTaxRate),
  (2, 5, @DPtoGoldPoints * @ConvertTaxRate),
  (2, 6, @DPtoFLAP * @ConvertTaxRate),
  (3, 2, 1/@DPtoHyperCoin),
  (3, 1, @DPtoGold/@DPtoHyperCoin),
  (3, 4, @DPtoHTML5/@DPtoHyperCoin * @ConvertTaxRate),
  (3, 5, @DPtoGoldPoints/@DPtoHyperCoin * @ConvertTaxRate),
  (3, 6, @DPtoFLAP/@DPtoHyperCoin * @ConvertTaxRate),
  (4, 2, 1/@DPtoHTML5),
  (4, 1, @DPtoGold/@DPtoHTML5),
  (4, 3, @DPtoHyperCoin/@DPtoHTML5 * @ConvertTaxRate),
  (4, 5, @DPtoGoldPoints/@DPtoHTML5 * @ConvertTaxRate),
  (4, 6, @DPtoFLAP/@DPtoHTML5 * @ConvertTaxRate),
  (5, 2, 1/@DPtoGoldPoints),
  (5, 1, @DPtoGold/@DPtoGoldPoints),
  (5, 3, @DPtoHyperCoin/@DPtoGoldPoints * @ConvertTaxRate),
  (5, 4, @DPtoHTML5/@DPtoGoldPoints * @ConvertTaxRate),
  (5, 6, @DPtoFLAP/@DPtoGoldPoints * @ConvertTaxRate),
  (6, 2, 1/@DPtoFLAP),
  (6, 1, @DPtoGold/@DPtoFLAP),
  (6, 3, @DPtoHyperCoin/@DPtoFLAP * @ConvertTaxRate),
  (6, 4, @DPtoHTML5/@DPtoFLAP * @ConvertTaxRate),
  (6, 5, @DPtoGoldPoints/@DPtoFLAP * @ConvertTaxRate)
) 
AS Source (SourceCurrencyId, DestinationCurrencyID, SourceMultiplier) 
ON Target.SourceCurrencyId = Source.SourceCurrencyId and Target.DestinationCurrencyId = Source.DestinationCurrencyId
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET SourceMultiplier = Source.SourceMultiplier
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (SourceCurrencyId, DestinationCurrencyID, SourceMultiplier) 
VALUES (SourceCurrencyId, DestinationCurrencyID, SourceMultiplier) 
-- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;