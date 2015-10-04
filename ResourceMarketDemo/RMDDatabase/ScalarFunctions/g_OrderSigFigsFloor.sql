CREATE FUNCTION [dbo].[g_OrderSigFigsFloor]
(
	@value float(53)
)
RETURNS float(53) --returns the submitted value rounded DOWN to 7 significant figures
AS
BEGIN
	if @value = 0
		RETURN 0
		
	RETURN round(@value, 6 - floor(log10(@value)), 1) -- @value should always be non-negative and if not then we should throw an error anyways.
END
