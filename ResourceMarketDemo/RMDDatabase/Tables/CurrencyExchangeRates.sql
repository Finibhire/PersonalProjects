CREATE TABLE [dbo].[CurrencyExchangeRates]
(
	[SourceCurrencyId] TINYINT NOT NULL , 
    [DestinationCurrencyId] TINYINT NOT NULL, 
    [SourceMultiplier] FLOAT(53) NOT NULL, 
    PRIMARY KEY ([SourceCurrencyId], [DestinationCurrencyId]), 
    CONSTRAINT [CK_CurrencyExchangeRates_IdsNotSame] CHECK ([SourceCurrencyId] <> [DestinationCurrencyId]), 
    CONSTRAINT [FK_CurrencyExchangeRates_CurrencyTypes_SourceId] FOREIGN KEY ([SourceCurrencyId]) REFERENCES [CurrencyTypes]([Id]),
    CONSTRAINT [FK_CurrencyExchangeRates_CurrencyTypes_DestinationId] FOREIGN KEY ([DestinationCurrencyId]) REFERENCES [CurrencyTypes]([Id])
)
