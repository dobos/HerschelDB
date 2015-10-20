BULK INSERT [load].[Observation]
FROM '[$datafile]'
WITH 
( 
   DATAFILETYPE = 'char',
   FIELDTERMINATOR = '|',
   ROWTERMINATOR = '\n',
   TABLOCK
)