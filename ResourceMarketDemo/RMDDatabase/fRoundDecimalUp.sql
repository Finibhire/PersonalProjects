CREATE FUNCTION [dbo].[fRoundDecimalUp]
(
	@input decimal(38,9),
	@MaxScale int
)
RETURNS decimal(38,9)
AS
BEGIN
	declare @roundNumbers decimal(38,9)
	declare @decimals decimal(38,9)
	declare @power decimal(38,9)
	set @roundNumbers = floor(@input)
	set @power = 10
	set @power = power(@power, @MaxScale)
	set @decimals = ceiling((@input - @roundNumbers) * @power) / @power
	return (@roundNumbers + @decimals)
END
