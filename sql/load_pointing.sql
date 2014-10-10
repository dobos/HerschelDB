BULK INSERT [load].[RawPointing]
FROM '\\retdb01.vo.elte.hu\Data\Temp\vo\everebelyi\0_pp.dat'
WITH 
( 
   DATAFILETYPE = 'char',
   FIELDTERMINATOR = ' ',
   ROWTERMINATOR = '\n',
   TABLOCK
)