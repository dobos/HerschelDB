# Download all PACS photo pointing files and store int pointing/pacs/photo/MyTable_[obsid].txt

table = asciiTableReader(file="list/pacs_photo.txt", tableType="CSV")
col = table.getColumn(0).data

for i in range(len(col)):
	obs = getObservation(obsid=col[i],useHsa=True)
	pp=obs.auxiliary.pointing
	calTree=getCalTree(obs=obs)
	orbitEphem = obs.auxiliary.orbitEphemeris
	frames = obs.level1.refs["HPPAVGB"].product
	frames = photAddInstantPointing(frames, pp, calTree=calTree, orbitEphem=orbitEphem, copy=1)
	pointing = frames.refs[0].product["Status"]
	formatter = CsvFormatter(delimiter=' ')
	asciiTableWriter(table=pointing, file='pointing/pacs/photo/myTable_'+str(col[i])+'.txt', formatter=formatter)
	del(obs,pp,calTree,orbitEphem,frames, pointing)
	System.gc()
