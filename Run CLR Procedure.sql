USE [Demo]
GO

DECLARE	@return_value int

EXEC	@return_value = [dbo].[SendSMS]
		@phoneNumber = N'447123123123',
		@from = N'SQLCLR',
		@message = N'A test message'

SELECT	'Return Value' = @return_value

GO
