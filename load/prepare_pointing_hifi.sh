#!/bin/bash

source ./settings.sh

INDIR="$HERSCHEL_SOURCE/Raw"
OUTDIR="$HERSCHEL_TEMP/pointing"

tail "$INDIR/obs/hifi_2.txt" -n+5 | awk '
{
	if ($11 != "0")
	{
		width = $11 * $14
		height = $12 * $15
		ra = $2
		dec = $3
		
		print $1, $6, "-", ra, dec, $7, $8, width, height
	}
	else if ($4 == "0.0" && $5 == "0.0")
	{
		print $1, $6, "-", $2, $3, $7, $8, "0", "0"
	}
	else
	{
		print $1, $6, "H", $2, $3, $7, $8, "0", "0"
		print $1, $6, "V", $4, $5, $7, $8, "0", "0"
	}
}
' > "$OUTDIR/hifi_pointing.dat"

tail "$INDIR/pointing/hifi/hifi_angle.txt" -n+1 | awk '
BEGIN { FS = ";"; OFS = "\t"; };

{
	print $1, $2, $3, $4=="null"?-1:$4
}
' > "$OUTDIR/hifi_angle.dat"