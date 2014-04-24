BULK INSERT [load].[RawPointing]
FROM '[$datafile]'
WITH 
( 
   DATAFILETYPE = 'char',
   FIELDTERMINATOR = ' ',
   ROWTERMINATOR = '\n'
)