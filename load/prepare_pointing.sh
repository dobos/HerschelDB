#!/bin/bash

source ./settings.sh

INDIR="$HERSCHEL_SOURCE/Raw/pointing"
OUTDIR="$HERSCHEL_TEMP/pointing"

#PACS

../bin/hload.exe prepare pointing pacs 1 "$INDIR/pacs/photo/myTable_*.txt" "$OUTDIR/pacs_photo_{0}.dat" 16
../bin/hload.exe prepare pointing pacs 1 "$INDIR/pacs/parallel/myTable_*.txt" "$OUTDIR/pacs_parallel_{0}.dat" 16
../bin/hload.exe prepare pointing pacs 4 "$INDIR/pacs/spectro_line/myTable_*.txt" "$OUTDIR/pacs_linespec_{0}.dat" 16
../bin/hload.exe prepare pointing pacs 2 "$INDIR/pacs/spectro_range/myTable_*.txt" "$OUTDIR/pacs_rangespec_{0}.dat" 16

#SPIRE

../bin/hload.exe prepare pointing spire 1 "$INDIR/spire/photo_large/myTable_*.txt" "$OUTDIR/spire_large_{0}.dat" 16
../bin/hload.exe prepare pointing spire 2 "$INDIR/spire/photo_small/myTable_*.txt" "$OUTDIR/spire_small_{0}.dat" 16
../bin/hload.exe prepare pointing spire 3 "$INDIR/spire/photo_fail/myTable_*.txt" "$OUTDIR/spire_fail_{0}.dat" 16
../bin/hload.exe prepare pointing spire 3 "$INDIR/spire/parallel/myTable_*.txt" "$OUTDIR/spire_parallel_{0}.dat" 16
../bin/hload.exe prepare pointing spire 4 "$INDIR/spire/spectro1/myTable_*.txt" "$OUTDIR/spire_spectro1_{0}.dat" 16
../bin/hload.exe prepare pointing spire 32 "$INDIR/spire/spectro_raster/myTable_*.txt" "$OUTDIR/spire_spectro_raster_{0}.dat" 16

#HIFI

../bin/hload.exe prepare pointing hifi 1 "$INDIR/hifi/point/myTable_*.txt" "$OUTDIR/hifi_pointing_{0}.dat" 16
../bin/hload.exe prepare pointing hifi 2 "$INDIR/hifi/scan/myTable_*.txt" "$OUTDIR/hifi_scan_{0}.dat" 16
../bin/hload.exe prepare pointing hifi 4 "$INDIR/hifi/map/myTable_*.txt" "$OUTDIR/hifi_map_{0}.dat" 16
