# Download all PACS photo headers and store in obs/pacs_photo.txt

table = asciiTableReader(file= CWD + "list/pacs_photo.txt", tableType="SPACES")
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
repT=Double1d()
sourceT=String1d()
obsST=String1d()
objectT=String1d()
mssT=String1d()

def LoadObs(id):
	obs = getObservation(obsid=id,useHsa=True)
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
		rep=obs.meta['repFactor'].value
	except: 
		rep=0
	try:
		source=obs.meta['source'].value
	except:
		source='none'
	try:
		mss=obs.meta['mapScanSpeed'].value
	except:
		mss='none'
	obsState=obs.meta['obsState'].value
	object=obs.meta['object'].value
	obsid.append(id)
	blueT.append(blue)
	instT.append(inst)
	cusT.append(cusMode)
	obsT.append(obsMode)
	aorL.append(aor)
	aotT.append(aot)
	pmT.append(pointingMode)
	repT.append(rep)
	sourceT.append(source)
	obsST.append(obsState)
	objectT.append(object)
	mssT.append(mss)

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
tds.addColumn("pmRA", Column(pmraT))
tds.addColumn("pmDEC", Column(pmdecT))
tds.addColumn("repetition", Column(repT))
tds.addColumn("pointingMode", Column(pmT))
tds.addColumn("source", Column(sourceT))
tds.addColumn("obsState", Column(obsST))
tds.addColumn("Object", Column(objectT))
tds.addColumn("ScanSpeed", Column(mssT))
formatter = CsvFormatter(delimiter=' ')
asciiTableWriter(table=tds, file= CWD + 'obs/pacs_photo.txt', formatter=formatter)
