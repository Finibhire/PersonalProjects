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
	set @power = cast(10 as decimal(10,0))
	set @power = power(@power, @MaxScale)
	set @decimals = cast((@input - @roundNumbers) as decimal(9,9))
	set @wholeDecimals = cast(ceiling(@decimals * @power) as decimal(9,0))
	set @decimals = cast((@wholeDecimals / @power) as decimal(9,9))
	return cast((@roundNumbers + @decimals) as decimal(38,9))
END
