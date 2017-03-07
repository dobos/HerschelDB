# Downloads PACS/SPIRE parallel photo obsevation headers and stores in obs/pacs_parallel.txt

table = asciiTableReader(file= CWD + "list/parallel.txt", tableType="SPACES")
col = table.getColumn(0).data

obsid=Long1d()
blueT=String1d()
instT=String1d()
cusT=String1d()
obsT=String1d()
aorL=String1d()
aotT=String1d()
pmraT=Double1d()
pmdecT=Double1d()
pmT=String1d()
mapsize1T=Double1d()
mapsize2T=Double1d()
sourceT=String1d()
obsST=String1d()
objectT=String1d()
msrT=String1d()

def LoadObs(id):
	obs = getObservation(obsid=id,useHsa=True, instrument='PACS')
	try:
		blue=obs.meta['blue'].value
	except:
		blue='none'
	inst=obs.meta['instMode'].value
	cusMode=obs.meta['cusMode'].value
	obsMode=obs.meta['obsMode'].value
	aor=obs.meta['aorLabel'].value
	aot=obs.meta['aot'].value
	pointingMode=obs.meta['pointingMode'].value
	try:
		source=obs.meta['source'].value
	except:
		source='none'
	try:
		mapsize1=obs.meta['mapSize1'].value
		mapsize2=obs.meta['mapSize2'].value
	except:
		mapsize1=0.0
		mapsize2=0.0
	obsState=obs.meta['obsState'].value
	object=obs.meta['object'].value
	msr=obs.meta['mapScanRate'].value
	obsid.append(col[i])
	blueT.append(blue)
	instT.append(inst)
	cusT.append(cusMode)
	obsT.append(obsMode)
	aorL.append(aor)
	aotT.append(aot)
	pmT.append(pointingMode)
	mapsize1T.append(mapsize1)
	mapsize2T.append(mapsize2)
	msrT.append(msr)
	sourceT.append(source)
	obsST.append(obsState)
	objectT.append(object)

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
tds.addColumn("Blue", Column(blueT))
tds.addColumn("instMode", Column(instT))
tds.addColumn("cusMode", Column(cusT))
tds.addColumn("obsMode", Column(obsT))
tds.addColumn("AOR_Label", Column(aorL))
tds.addColumn("AOT", Column(aotT))
tds.addColumn("MapScanRate", Column(msrT))
tds.addColumn("MapSize1", Column(mapsize1T))
tds.addColumn("MapSize2", Column(mapsize2T))
tds.addColumn("pointingMode", Column(pmT))
tds.addColumn("source", Column(sourceT))
tds.addColumn("obsState", Column(obsST))
tds.addColumn("Object", Column(objectT))
formatter = CsvFormatter(delimiter=' ')
asciiTableWriter(table=tds, file= CWD + 'obs/pacs_parallel.txt', formatter=formatter)

  