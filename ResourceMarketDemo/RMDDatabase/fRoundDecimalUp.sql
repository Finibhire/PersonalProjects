CREATE FUNCTION [dbo].[fRoundDecimalUp]
(
	@input decimal(38,9),
	@MaxScale int
)
RETURNS decimal(38,9)
AS
BEGIN
	declare @roundNumbers decimal(29,0)
	declare @decimals decimal(9,9)
	declare @wholeDecimals decimal(9,0)
	declare @power decimal(10,0)
	set @roundNumbers = floor(@input)
	set @power = 10
	set @power = power(@power, @MaxScale)
	set @decimals = (@input - @roundNumbers)
	set @wholeDecimals = ceiling(@decimals * @power)
	set @decimals = @wholeDecimals / @power
	return (@roundNumbers + @decimals)
END
