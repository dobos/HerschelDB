# Download all SPIRE pointed spectro observation pointing files and store in pointing/spire/spectro/myTable_[obsid].txt

table = asciiTableReader(file="list/spire_spectro_pointed.txt", tableType="SPACES")
col = table.getColumn(0).data

for i in range(len(col)):
	raTotal = Double1d()
	decTotal = Double1d()
	paTotal = Double1d()
	timeTotal=Long1d()
	print col[i], i
	obs = getObservation(obsid=col[i],useHsa=True)
	p2 = obs
	time1=p2.meta['startDate'].time.microsecondsSince1958()
	time2=p2.meta['endDate'].time.microsecondsSince1958()
	posA=p2.meta['posAngle'].double
	ra=p2.meta['ra'].double
	dec=p2.meta['dec'].double
	obsid=p2.meta['obsid'].long
	
	timeTotal.append(time1)
	timeTotal.append(time2)
	raTotal.append(ra)
	raTotal.append(ra)
	decTotal.append(dec)
	decTotal.append(dec)
	paTotal.append(posA)
	paTotal.append(posA)
	
	tds = TableDataset()
	tds.addColumn("RA", Column(raTotal))
	tds.addColumn("DEC", Column(decTotal))
	tds.addColumn("PA",  Column(paTotal))
	tds.addColumn("Time",  Column(timeTotal))
	asciiTableWriter(table=tds, file='pointing/spire/spectro/myTable_'+str(obsid)+'.txt', formatter=CsvFormatter(delimiter=' '))
	print i, len(col), obsid
	del(obs,tds, raTotal, decTotal, paTotal, timeTotal)
	del(ra, dec, posA,  time1, time2)
	System.gc()