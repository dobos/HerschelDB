BULK INSERT [load].[RawObservation]
FROM '[$datafile]'
WITH 
( 
   DATAFILETYPE = 'char',
   FIELDTERMINATOR = '|',
   ROWTERMINATOR = '\n',
   TABLOCK
)