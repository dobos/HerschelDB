# Download PACS line spectro headers and store in obs/pacs_spectro_line.txt

#obsid=long(1342209001)
#obsid=long(1342205209)
#obsid=long(1342203096)

table = asciiTableReader(file="pacs_spectro_line.list", tableType="SPACES")
col = table.getColumn(0).data

obsid, nolT ,  norcT, norlT =Long1d(), Long1d(), Long1d(), Long1d()
lineT, instT, cusT, obsT=String1d(), String1d(), String1d(), String1d()
aorL,aotT, sourceT, pmT =String1d(), String1d(), String1d(), String1d()
raT, decT, paT ,pointST= Double1d(), Double1d(), Double1d(), Double1d()
obsST, objectT =String1d(), String1d()
chopNT = Bool1d()

for i in range(len(col)):
		print col[i], i
		obs = getObservation(obsid=col[i],useHsa=True)
		try:
			line1=obs.meta['Line1'].value
		except:
			line1='none'
		try:
			nol=obs.meta['Number of Lines'].value
		except:
			nol=0
		ra=obs.meta['ra'].value
		dec=obs.meta['dec'].value
		pa=obs.meta['posAngle'].value
		inst=obs.meta['instMode'].value
		cusMode=obs.meta['cusMode'].value
		obsMode=obs.meta['obsMode'].value
		aor=obs.meta['aorLabel'].value
		aot=obs.meta['aot'].value
		pointingMode=obs.meta['pointingMode'].value
		try:
			chopNod=obs.meta['chopNod'].value
		except:
			chopNod=False
		try:
			norc=obs.meta['numRasterCol'].value
			norl=obs.meta['numRasterLines'].value
			points=obs.meta['pointStep'].value
		except:
			norc=0L
			norl=0L
			points=0.0
		try:
			source=obs.meta['source'].value
		except:
			source='none'
		obsState=obs.meta['obsState'].value
		object=obs.meta['object'].value
		obsid.append(col[i])
		nolT.append(nol)
		lineT.append(line1)
		raT.append(ra)
		decT.append(dec)
		paT.append(pa)
		instT.append(inst)
		cusT.append(cusMode)
		obsT.append(obsMode)
		aorL.append(aor)
		aotT.append(aot)
		pmT.append(pointingMode)
		chopNT.append(chopNod)
		pointST.append(points)
		norlT.append(norl)
		norcT.append(norc)
		sourceT.append(source)
		obsST.append(obsState)
		objectT.append(object)
tds = TableDataset()
tds.addColumn("OBSID", Column(obsid))
tds.addColumn("RA", Column(raT))
tds.addColumn("DEC", Column(decT))
tds.addColumn("PA", Column(paT))
tds.addColumn("instMode", Column(instT))
tds.addColumn("cusMode", Column(cusT))
tds.addColumn("obsMode", Column(obsT))
tds.addColumn("AOR_Label", Column(aorL))
tds.addColumn("AOT", Column(aotT))
tds.addColumn("pointingMode", Column(pmT))
tds.addColumn("chopNod", Column(chopNT))
tds.addColumn("RasterMapPointStep(arcsec)", Column(pointST))
tds.addColumn("RasterLine", Column(norlT))
tds.addColumn("RasterColoumn", Column(norcT))
tds.addColumn("source", Column(sourceT))
tds.addColumn("obsState", Column(obsST))
tds.addColumn("Object", Column(objectT))
tds.addColumn("Number_of_Line", Column(nolT))
tds.addColumn("Line", Column(lineT))
asciiTableWriter(table=tds, file='obs/pacs_spectro_line.txt', formatter=formatter)

  
