#wait for the SQL Server to come up

/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -i init.sql
wait
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -i OKATO.sql
wait
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -i UnitCode.sql
wait
cp ./IP2LOCATION-LITE-DB5MY.CSV /var/opt/mssql/data/IP2LOCATION-LITE-DB5MY.CSV
wait
echo "load IP location to database...wait"
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -i IP.sql