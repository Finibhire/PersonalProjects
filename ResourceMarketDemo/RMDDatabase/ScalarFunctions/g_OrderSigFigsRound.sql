CREATE FUNCTION [dbo].[g_OrderSigFigsRound]
(
	@value float(53)
)
RETURNS float(53) --returns the submitted value rounded to 7 significant figures
AS
BEGIN
	if @value = 0
		RETURN 0
		
	RETURN round(@value, 6 - floor(log10(@value))) -- @value should always be non-negative and if not then we should throw an error anyways.
END
