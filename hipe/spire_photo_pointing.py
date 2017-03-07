table = asciiTableReader(file="spire_photo.list", tableType="SPACES")
col = table.getColumn(0).data

for j in range(len(col)):
	obsid = col[j]
	selectBolometer = 'PSWE8'
	obs = getObservation(obsid, useHsa=True, instrument='SPIRE')
	raTotal = Double1d()
	decTotal = Double1d()
	angVelTotal = Double1d()
	paTotal = Double1d()
	timeTotal = Double1d()
	corrtimeTotal = Double1d()
	# Loop through all the Level1 products (Pointed Scan Products or PSP)
	for i in range(obs.level1.getCount()):
		print "Scan Line %d/%d"%((i+1), obs.level1.getCount())
		psp = obs.level1.getProduct(i)
		ra = psp.getRa(selectBolometer)
		dec = psp.getDec(selectBolometer)
		angVel = psp.getAngVelocity() # same for all bolometers
		pa = psp.meta['posAngle'].value
		pa2 = Double1d(len(ra))
		pa2[0:len(ra)] = pa
		timeArr = psp['signal']['sampleTime'].data
		corrTimeArr = psp['signal']['corrTime'].data
		raTotal.append(ra)
		decTotal.append(dec)
		angVelTotal.append(angVel)
		paTotal.append(pa2)
		timeTotal.append(timeArr)
		corrtimeTotal.append(corrTimeArr)
	tds = TableDataset()
	tds.addColumn("RA", Column(raTotal))
	tds.addColumn("DEC", Column(decTotal))
	tds.addColumn("angVel",  Column(angVelTotal))
	tds.addColumn("PA",  Column(paTotal))
	tds.addColumn("sampleTime",  Column(timeTotal))
	tds.addColumn("corrTime",  Column(corrtimeTotal))
	asciiTableWriter(table=tds, file='myTable_'+str(obsid)+'.txt', formatter=CsvFormatter(delimiter=' '))
	print j, len(col), obsid
	del(obs,tds,raTotal, decTotal, angVelTotal,  corrtimeTotal, paTotal, timeTotal)
	del(ra, dec, angVel, corrTimeArr, pa, pa2, psp, timeArr)
	System.gc()
