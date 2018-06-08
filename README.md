# Example SMS sends for SQL Server using CLR Integration

## Security Notes
This example introduces assemblies to SQL server and relaxes the CLR security in order to allow SQL Server to run the code that contains dynamic types. This is a softening of SQL Servers security and therefore you should consider carefully whether this is something you want to do. This is purely an example and not a recommendation.

## Configuring the database for running the CLR code
Follow these instructions in order to prepare the database for running the CLR assembly:
1. Clone the Github repository
2. Update the file **SendSMS.cs** to add your API Space Id and JWT Access Token you created in the Comapi Portal as indicated at the top of the file
3. Build the example CLR DLL **comapi-quickstart-sql-clr-integration** in Visual Studio
4. Copy the compiled dll (*comapi-quickstart-sql-clr-integration.dll*) to your SQL Server
5. Copy the file **sqlservr.exe.config** to your SQL Server in the same folder as **sqlservr.exe** , check the path properties of the **Microsoft SQL Server** NT Service to find this e.g. **C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\Binn**
6. Update the file **CLR_SendSMS.sql** to use the path to **comapi-quickstart-sql-clr-integration.dll** on your SQL Server you copied in step 4. as directed in the file
7. Run the script **CLR_SendSMS.sql** , this will configure the database and register the CLR assembly and create a stored procedure to access it
8. Restart the SQL Server instance to ensure that the security settings are applied
9. Update the script **Run CLR Procedure.sql** to use your test mobile number in international format e.g. For UK 07123 123123 would become 447123123123
10. Run the script **Run CLR Procedure.sql** and you should receive an SMS shortly afterwards

## More information
Find out more about how Comapi can help you send business messaging across multiple channels with ease at:
* [comapi.com](http://comapi.com)
* [docs.comapi.com](http://docs.comapi.com)