BULK INSERT [Pointing]
FROM 'c:\Data\Temp\vo\herschel\test.dat' -- '[$datafile]'
WITH 
( 
   BATCHSIZE = 10000,
   DATAFILETYPE = 'char',
   FIELDTERMINATOR = ' ',
   ROWTERMINATOR = '\n',
   TABLOCK
)
