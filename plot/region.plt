set auto

region = '< ..\bin\Debug\hplot.exe %s "%s"'
sql = sprintf('SELECT region FROM LegRegion WHERE obsID = %d', $0)

plot sprintf(region, 'outline', sql) w l