
table = asciiTableReader(file="hifi_list.dat", tableType="SPACES")
col = table.getColumn(0).data
obsidT=Long1d()
bandT=String1d()
instT=String1d()
cusT=String1d()
obsT=String1d()
aorL=String1d()
aotT=String1d()
pmT=String1d()
sourceT=String1d()
obsST=String1d()
objectT=String1d()
msrT=String1d()
w=Double1d()
h=Double1d()
pattangT=Double1d()
hpbwT=Double1d()


for i in range(len(col)):
		print col[i], i
		obsid=col[i]
		obs = getObservation(obsid=obsid,useHsa=True, instrument='HIFI')
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
			band=obs.meta['Band'].value
		except:
			band='none'
		obsState=obs.meta['obsState'].value
		object=obs.meta['object'].value
		try:  
		  print obs.level1.refs['HRS-H'].product.meta['mapWidthCommanded']
		except:
		  mw=0
		  mh=0
		  pattang=0
		else:
		  mw=obs.level1.refs['HRS-H'].product.meta['mapWidthCommanded'].value
		  mh=obs.level1.refs['HRS-H'].product.meta['mapHeightCommanded'].value
		  pattang=obs.level1.refs["HRS-H"].product.meta['pattAngle'].value
		try: 
		  hpbw=obs.refs["level2"].product.refs["HRS-H-LSB"].product.meta['hpbw'].double
		except:
		  hpbw=0.
		obsidT.append(col[i])
		bandT.append(str(band))
		instT.append(inst)
		cusT.append(cusMode)
		obsT.append(obsMode)
		aorL.append(aor)
		aotT.append(aot)
		pmT.append(pointingMode)
		sourceT.append(source)
		obsST.append(obsState)
		objectT.append(object)
		w.append(mw)
		h.append(mh)
		pattangT.append(pattang)
		hpbwT.append(hpbw)
tds = TableDataset()
tds = TableDataset()
tds.addColumn("OBSID", Column(obsidT))
tds.addColumn("Band", Column(bandT))
tds.addColumn("instMode", Column(instT))
tds.addColumn("cusMode", Column(cusT))
tds.addColumn("obsMode", Column(obsT))
tds.addColumn("AOR_Label", Column(aorL))
tds.addColumn("AOT", Column(aotT))
tds.addColumn("pointingMode", Column(pmT))
tds.addColumn("source", Column(sourceT))
tds.addColumn("obsState", Column(obsST))
tds.addColumn("Object", Column(objectT))
tds.addColumn("Map_width", Column(w))
tds.addColumn("Map_height", Column(h))
tds.addColumn("Pattangle", Column(pattangT))
tds.addColumn("HPBW", Column(hpbwT))
formatter = CsvFormatter(delimiter=' ')
asciiTableWriter(table=tds, file='hifi_header.txt', formatter=formatter)

  
