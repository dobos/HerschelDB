# Download HIFI extended headers that stores beam positions for H/V and
# store in pointing/hifi/hifi.txt

raTotal1, raTotal2, decTotal1, decTotal2 = Double1d(), Double1d(), Double1d(), Double1d()
bandTotal, ctype1Total, ctype2Total, ctype3Total, cunit1Total, cunit2Total, cunit3Total  = String1d(), String1d(), String1d(), String1d(), String1d(), String1d(), String1d()
PMTotal, CMTotal = String1d(), String1d()
startTTotal, endTTotal, cdelt1Total, cdelt2Total, cdelt3Total, crpix1Total, crpix2Total, crpix3Total = Long1d(), Long1d(), Double1d(), Double1d(), Double1d(), Double1d(), Double1d(), Double1d()
crval1Total, crval2Total, crval3Total, naxis1Total, naxis2Total, naxis3Total =  Double1d(), Double1d(), Double1d(), Long1d(), Long1d(), Long1d()
crota2Total = Double1d()
obsTotal=String1d()

nu_0 = [480., 640., 800., 960., 1120., 1410., 1910.]
HPBW_nu0 = [43.3,32.85,26.05,21.8,19.5,14.8,11.1]

table = asciiTableReader(file="hifi.list", tableType="SPACES")
col = table.getColumn(0).data

for j in range(1, len(col)):
	obsid = col[j]
	obs = getObservation(obsid, useHsa=True)
	band, ab = obs.meta['Band'].string
	PM = obs.meta['pointingMode'].value
	CM = obs.meta['cusMode'].value
	endDate = obs.meta['endDate'].getTime().microsecondsSince1958()
	startDate = obs.meta['startDate'].getTime().microsecondsSince1958()
	print obsid, band, PM
	try:
		h = obs.refs["level2_5"].product.refs["spectrum"].product.refs["spectrum_WBS-H-USB"].product["dataset"]
		v = obs.refs["level2_5"].product.refs["spectrum"].product.refs["spectrum_WBS-V-USB"].product["dataset"]
		naxis1, naxis2, naxis3=0, 0, 0
		cdelt1, cdelt2, cdelt3=0, 0, 0
		crpix1, crpix2, crpix3=0, 0, 0
		crval1, crval2, crval3=0, 0, 0
		ctype1, ctype2, ctype3='0', '0', '0'
		cunit1, cunit2, cunit3='0', '0', '0'
		crota2=0
		hRA, hDEC = h.meta["ra"].getDouble(),h.meta["dec"].getDouble()
		vRA, vDEC = v.meta["ra"].getDouble(),v.meta["dec"].getDouble()
	except:
		try:
			h = obs.refs["level2_5"].product.refs["myDecon"].product.refs["myDecon_WBS-H"].product
			v = obs.refs["level2_5"].product.refs["myDecon"].product.refs["myDecon_WBS-V"].product
			naxis1, naxis2, naxis3=0, 0, 0
			cdelt1, cdelt2, cdelt3=0, 0, 0
			crpix1, crpix2, crpix3=0, 0, 0
			crval1, crval2, crval3=0, 0, 0
			ctype1, ctype2, ctype3='0', '0', '0'
			cunit1, cunit2, cunit3='0', '0', '0'
			crota2=0
			hRA, hDEC = h.meta["ra"].getDouble(),h.meta["dec"].getDouble()
			vRA, vDEC = v.meta["ra"].getDouble(),v.meta["dec"].getDouble()
		except:
			try:
				h = obs.refs["level2_5"].product.refs["cubesContext"].product.refs["cubesContext_WBS-H-USB"].product.refs["cube_WBS_H_USB_1"].product
				v = obs.refs["level2_5"].product.refs["cubesContext"].product.refs["cubesContext_WBS-V-USB"].product.refs["cube_WBS_V_USB_1"].product
				hw, hh = h.meta["mapWidthGridded"].getDouble(),h.meta["mapHeightGridded"].getDouble()
				vw, vh = v.meta["mapWidthGridded"].getDouble(),v.meta["mapHeightGridded"].getDouble()
				image=obs.refs["level2_5"].product.refs["cubesContext"].product.refs["cubesContext_WBS-H-USB"].product.refs["cube_WBS_H_USB_1"].product["image"]
				naxis1, naxis2, naxis3=image.meta['naxis1'].value, image.meta['naxis2'].value, image.meta['naxis3'].value
				cdelt1, cdelt2, cdelt3=image.meta['cdelt1'].value, image.meta['cdelt2'].value, image.meta['cdelt3'].value
				crpix1, crpix2, crpix3=image.meta['crpix1'].value, image.meta['crpix2'].value, image.meta['crpix3'].value
				crval1, crval2, crval3=image.meta['crval1'].value, image.meta['crval2'].value, image.meta['crval3'].value
				ctype1, ctype2, ctype3=image.meta['ctype1'].value, image.meta['ctype2'].value, image.meta['ctype3'].value
				cunit1, cunit2, cunit3=image.meta['cunit1'].value, image.meta['cunit2'].value, image.meta['cunit3'].value
				crota2=image.meta['crota2'].value
				hRA, hDEC = obs.meta["ra"].getDouble(),obs.meta["dec"].getDouble()
				vRA, vDEC = 0.0,0.0
			except:
				naxis1, naxis2, naxis3=0, 0, 0
				cdelt1, cdelt2, cdelt3=0, 0, 0
				crpix1, crpix2, crpix3=0, 0, 0
				crval1, crval2, crval3=0, 0, 0
				ctype1, ctype2, ctype3='0', '0', '0'
				cunit1, cunit2, cunit3='0', '0', '0'
				crota2=0
				hRA, hDEC = obs.meta["ra"].getDouble(),obs.meta["dec"].getDouble()
				vRA, vDEC = 0.0,0.0
	raTotal1.append(hRA)
	decTotal1.append(hDEC)
	raTotal2.append(vRA)
	decTotal2.append(vDEC)
	bandTotal.append(band)
	startTTotal.append(startDate)
	endTTotal.append(endDate)
	PMTotal.append(PM)
	CMTotal.append(CM)
	naxis1Total.append(naxis1)
	naxis2Total.append(naxis2)
	naxis3Total.append(naxis3)
	cdelt1Total.append(cdelt1)
	cdelt2Total.append(cdelt2)
	cdelt3Total.append(cdelt3)
	crpix1Total.append(crpix1)
	crpix2Total.append(crpix2)
	crpix3Total.append(crpix3)
	crval1Total.append(crval1)
	crval2Total.append(crval2)
	crval3Total.append(crval3)
	ctype1Total.append(ctype1)
	ctype2Total.append(ctype2)
	ctype3Total.append(ctype3)
	cunit1Total.append(cunit1)
	cunit2Total.append(cunit2)
	cunit3Total.append(cunit3)
	crota2Total.append(crota2)
	obsTotal.append(obsid)
tds = TableDataset()
tds.addColumn("obsID",  Column(obsTotal))
tds.addColumn("H_RA", Column(raTotal1))
tds.addColumn("H_DEC", Column(decTotal1))
tds.addColumn("V_RA", Column(raTotal2))
tds.addColumn("V_DEC", Column(decTotal2))
tds.addColumn("Band", Column(bandTotal))
tds.addColumn("startTime",  Column(startTTotal))
tds.addColumn("endTime",  Column(endTTotal))
tds.addColumn("PointingMode",  Column(PMTotal))
tds.addColumn("cusmode",  Column(CMTotal))
tds.addColumn("naxis1",  Column(naxis1Total))
tds.addColumn("naxis2",  Column(naxis2Total))
tds.addColumn("naxis3",  Column(naxis3Total))
tds.addColumn("cdelt1",  Column(cdelt1Total))
tds.addColumn("cdelt2",  Column(cdelt2Total))
tds.addColumn("cdelt3",  Column(cdelt3Total))
tds.addColumn("crpix1",  Column(crpix1Total))
tds.addColumn("crpix2",  Column(crpix2Total))
tds.addColumn("crpix3",  Column(crpix3Total))
tds.addColumn("crval1",  Column(crval1Total))
tds.addColumn("crval2",  Column(crval2Total))
tds.addColumn("crval3",  Column(crval3Total))
tds.addColumn("ctype1",  Column(ctype1Total))
tds.addColumn("ctype2",  Column(ctype2Total))
tds.addColumn("ctype3",  Column(ctype3Total))
tds.addColumn("cunit1",  Column(cunit1Total))
tds.addColumn("cunit2",  Column(cunit2Total))
tds.addColumn("cunit3",  Column(cunit3Total))
tds.addColumn("crota2",  Column(crota2Total))
asciiTableWriter(table=tds, file='pointing/hifi/hifi.txt', formatter=CsvFormatter(delimiter=' '))
