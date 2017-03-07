# Download SPIRE photo headers and store in obs/spire_photo.txt

table = asciiTableReader(file= CWD + "list/spire_photo.txt", tableType="SPACES")
col = table.getColumn(0).data

obsid, numPT, norT =Long1d(), Long1d(), Long1d()
instT, cusT, obsT= String1d(), String1d(), String1d()
aorL,aotT, sourceT, pmT =String1d(), String1d(), String1d(), String1d()
raT, decT, paT, mapLT, mapHT= Double1d(), Double1d(), Double1d(), Double1d(), Double1d()
obsST, objectT, apT, mapsT, crT =String1d(), String1d(), String1d(), String1d(), String1d()

def LoadObs(id):
	obs = getObservation(obsid=id,useHsa=True)
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
		aperture=obs.meta['aperture'].value
	except:
		aperture='none'
	try:
		mapsampling=obs.meta['mapSampling'].value
	except:
		mapsampling='none'
	try:
		nor=obs.meta['numRepetitions'].value
	except:
		nor=0
	try:
		cr=obs.meta['commandedResolution'].value
	except:
		cr='none'
	try:
		mapL=obs.meta['mapLength'].value
		mapH=obs.meta['mapHeight'].value
	except:
		mapL=0.0
		mapH=0.0
	try:
		source=obs.meta['source'].value
	except:
		source='none'
	obsState=obs.meta['obsState'].value
	object=obs.meta['object'].value
	obsid.append(id)
	raT.append(ra)
	decT.append(dec)
	paT.append(pa)
	instT.append(inst)
	cusT.append(cusMode)
	obsT.append(obsMode)
	aorL.append(aor)
	aotT.append(aot)
	pmT.append(pointingMode)
	apT.append(aperture)
	mapsT.append(mapsampling)
	mapLT.append(mapL)
	mapHT.append(mapH)
	sourceT.append(source)
	obsST.append(obsState)
	objectT.append(object)
	norT.append(nor)
	crT.append(cr)

q = 0
for i in range(START, len(col)):
	print col[i], i
	id=col[i]
	try:
		LoadObs(id)
		q = q + 1
		if q == LIMIT:
			print "Limit reached, exiting."
			break
	except herschel.ia.task.TaskException as e:
		print e

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
tds.addColumn("aperture", Column(apT))
tds.addColumn("mapSamling", Column(mapsT))
tds.addColumn("mapLength", Column(mapLT))
tds.addColumn("mapHeight", Column(mapHT))
tds.addColumn("obsState", Column(obsST))
tds.addColumn("Source", Column(sourceT))
tds.addColumn("Object", Column(objectT))
tds.addColumn("number of repetition", Column(norT))
tds.addColumn("Commanded Resolution", Column(crT))
formatter = CsvFormatter(delimiter=' ')
asciiTableWriter(table=tds, file= CWD + 'obs/spire_photo.txt', formatter=formatter)

  