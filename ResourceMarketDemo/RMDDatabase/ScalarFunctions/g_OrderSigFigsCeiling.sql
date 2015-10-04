CREATE FUNCTION [dbo].[g_OrderSigFigsCeiling]
(
	@value float(53)
)
RETURNS float(53) --returns the submitted value rounded UP to 7 significant figures
AS
BEGIN
	if @value = 0
		RETURN 0
	
	declare @pow float(53) = power(cast(10 as float(53)), 6 - floor(log10(@value)))

	RETURN ceiling(@value * @pow) / @pow
END
