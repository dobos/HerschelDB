# Download all SPIRE raster spectro observation pointing files and store in pointing/spire/spectro/myTable_[obsid].txt

table = asciiTableReader(file="spire_spectro_raster.list", tableType="CSV")
col = table.getColumn(0).data



for i in range(len(col)):
	raTotal = Double1d()
	decTotal = Double1d()
	paTotal = Double1d()
	startTotal=Long1d()
	endTotal=Long1d()
	bbidTotal=Long1d()
	nR=Int1d()
	nP=Int1d()
	print col[i], i
	obs = getObservation(obsid=col[i],useHsa=True)
	p2 = obs
	obsid=col[i]
	posA=p2.meta['posAngle'].double
	numR=obs.meta['numRepetitions'].value
	numP=obs.meta['numPoints'].value
	for k in range(numR):
		for j in  range(numP):
			try:
				ra=obs.refs["level1"].product.refs["Point_"+str(j)+"_Jiggle_"+str(k)+"_HR"].product.refs["interferogram"].product.meta['ra'].value
				dec=obs.refs["level1"].product.refs["Point_"+str(j)+"_Jiggle_"+str(k)+"_HR"].product.refs["interferogram"].product.meta['dec'].value
				bbid=obs.refs["level1"].product.refs["Point_"+str(j)+"_Jiggle_"+str(k)+"_HR"].product.refs["interferogram"].product.meta['bbid'].value
				startD=obs.refs["level1"].product.refs["Point_"+str(j)+"_Jiggle_"+str(k)+"_HR"].product.refs["interferogram"].product.meta['startDate'].time.microsecondsSince1958()
				endD=obs.refs["level1"].product.refs["Point_"+str(j)+"_Jiggle_"+str(k)+"_HR"].product.refs["interferogram"].product.meta['endDate'].time.microsecondsSince1958()
				obsid=p2.meta['obsid'].long
				startTotal.append(startD)
				startTotal.append(endD)
				raTotal.append(ra)
				raTotal.append(ra)
				decTotal.append(dec)
				decTotal.append(dec)
				paTotal.append(posA)
				paTotal.append(posA)
				bbidTotal.append(bbid)
				bbidTotal.append(bbid)
				nR.append(k)
				nR.append(k)
				nP.append(j)
				nP.append(j)
			except:
				try:
					ra=obs.refs["level1"].product.refs["Point_"+str(j)+"_Jiggle_"+str(k)+"_LR"].product.refs["interferogram"].product.meta['ra'].value
					dec=obs.refs["level1"].product.refs["Point_"+str(j)+"_Jiggle_"+str(k)+"_LR"].product.refs["interferogram"].product.meta['dec'].value
					bbid=obs.refs["level1"].product.refs["Point_"+str(j)+"_Jiggle_"+str(k)+"_LR"].product.refs["interferogram"].product.meta['bbid'].value
					startD=obs.refs["level1"].product.refs["Point_"+str(j)+"_Jiggle_"+str(k)+"_LR"].product.refs["interferogram"].product.meta['startDate'].time.microsecondsSince1958()
					endD=obs.refs["level1"].product.refs["Point_"+str(j)+"_Jiggle_"+str(k)+"_LR"].product.refs["interferogram"].product.meta['endDate'].time.microsecondsSince1958()
					obsid=p2.meta['obsid'].long
					startTotal.append(startD)
					startTotal.append(endD)
					raTotal.append(ra)
					raTotal.append(ra)
					decTotal.append(dec)
					decTotal.append(dec)
					paTotal.append(posA)
					paTotal.append(posA)
					bbidTotal.append(bbid)
					bbidTotal.append(bbid)
					nR.append(k)
					nR.append(k)
					nP.append(j)
					nP.append(j)
				except:
					continue
	tds = TableDataset()
	tds.addColumn("RA", Column(raTotal))
	tds.addColumn("DEC", Column(decTotal))
	tds.addColumn("PA",  Column(paTotal))
	tds.addColumn("StartTime",  Column(startTotal))
	tds.addColumn("bbidTime",  Column(bbidTotal))
	tds.addColumn("Point",  Column(nP))
	tds.addColumn("Repetition",  Column(nR))
	asciiTableWriter(table=tds, file='pointing/spire/spectro/myTable_'+str(obsid)+'.txt', formatter=CsvFormatter(delimiter=' '))
	print i, len(col), obsid
	try:
		del(obs, tds, raTotal, decTotal, paTotal, startTotal, nP, nR, bbidTotal)
		del(ra, dec, posA, numR, numP)
	except:
		continue
	System.gc()