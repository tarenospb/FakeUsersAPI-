USE FakeUsersApi
GO
BULK INSERT [Ip2Location]
FROM '/var/opt/mssql/data/IP2LOCATION-LITE-DB5MY.CSV'
WITH
(
	FIELDTERMINATOR = ';',
	ROWTERMINATOR = '|'
)
GO