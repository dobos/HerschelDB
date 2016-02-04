
table = asciiTableReader(file="pacs.linespec", tableType="CSV")
col = table.getColumn(0).data

for i in range(len(col)):
	print col[i], i
	obs = getObservation(obsid=col[i],useHsa=True)
	pp=obs.auxiliary.pointing
	calTree=getCalTree(obs=obs)
	orbitEphem = obs.auxiliary.orbitEphemeris
        slicedFrames  = SlicedFrames(obs.level0.fitted.getCamera('blue').product)
        slicedFrames = specAddInstantPointing(slicedFrames, pp, calTree = calTree, orbitEphem = orbitEphem , horizonsProduct = obs.auxiliary.horizons)    
	pointing = slicedFrames.refs[0].product["Status"]
	formatter = CsvFormatter(delimiter=' ')
	asciiTableWriter(table=pointing, file='myTable_'+str(col[i])+'.txt', formatter=formatter)
	del(obs,pp,calTree,orbitEphem,slicedFrames, pointing)
	System.gc()
  
