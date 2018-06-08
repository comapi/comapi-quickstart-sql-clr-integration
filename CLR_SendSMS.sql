-- Enable CLR on the database
sp_configure 'show advanced options', 1;  
GO  
RECONFIGURE;  
GO  
sp_configure 'clr enabled', 1;  
GO  
RECONFIGURE;  
GO  

-- Set the datbase to trustworthy to resolve issues with security restrictions for the CLR, ensure you change the database name 'Demo' to your database instance
ALTER DATABASE Demo SET TRUSTWORTHY ON

-- Register assemblies to allow the code to run
IF NOT EXISTS (SELECT name FROM sys.assemblies WHERE name = 'System_Runtime_Serialization')
BEGIN
    CREATE ASSEMBLY System_Runtime_Serialization FROM 'C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Runtime.Serialization.dll'
    WITH PERMISSION_SET = UNSAFE
END
GO

IF NOT EXISTS (SELECT name FROM sys.assemblies WHERE name = 'System_Web')
BEGIN
    CREATE ASSEMBLY System_Web FROM 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Web.dll'
    WITH PERMISSION_SET = UNSAFE
END
GO

IF NOT EXISTS (SELECT name FROM sys.assemblies WHERE name = 'Microsoft_CSharp')
BEGIN
    CREATE ASSEMBLY Microsoft_CSharp FROM 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Microsoft.CSharp.dll'
    WITH PERMISSION_SET = UNSAFE
END
GO

IF EXISTS (SELECT name FROM sys.assemblies WHERE name = 'comapiOneApi')  
   drop assembly comapiOneApi

-- Note: Update the file path below to point to the compiled comapi-quickstart-sql-clr-integration.dll file on your server.
CREATE ASSEMBLY comapiOneApi from 'D:\Work\GitHub\comapi-quickstart-sql-clr-integration\comapi-quickstart-sql-clr-integration\bin\Debug\comapi-quickstart-sql-clr-integration.dll' 
WITH PERMISSION_SET = UNSAFE
GO

SELECT * FROM sys.assemblies 
GO

-- Create a stored proc linked to the comapiOneApi CLR assembly we just registered
IF EXISTS (SELECT name FROM sysobjects WHERE name = 'SendSMS')  
   drop procedure SendSMS
GO

CREATE PROCEDURE SendSMS  
(
@phoneNumber NVARCHAR(20),
@from NVARCHAR(20),
@message NVARCHAR(4000) )
AS  
EXTERNAL NAME comapiOneApi.ComapiOneApi.Send  

GO
