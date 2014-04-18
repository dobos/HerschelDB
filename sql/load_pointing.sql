BULK INSERT [PointingTemp]
FROM '[$datafile]'
WITH 
( 
   DATAFILETYPE = 'char',
   FIELDTERMINATOR = ' ',
   ROWTERMINATOR = '\n',
   TABLOCK
)