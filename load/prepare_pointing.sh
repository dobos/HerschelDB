#!/bin/bash

#PACS

INDIR="//blackhole/data/Raid6_2/vo/Herschel/Raw/pointing"
OUTDIR="../pointing"

#./hload.exe prepare pointing pacs 1 "$INDIR/pacs/photo/myTable_*.txt" "$OUTDIR/pacs_photo_{0}.dat" 16

#./hload.exe prepare pointing pacs 1 "$INDIR/pacs/parallel/myTable_*.txt" "$OUTDIR/pacs_parallel_{0}.dat" 16

#./hload.exe prepare pointing pacs 4 "$INDIR/pacs/spectro_line/myTable_*.txt" "$OUTDIR/pacs_linespec_{0}.dat" 16

#./hload.exe prepare pointing pacs 2 "$INDIR/pacs/spectro_range/myTable_*.txt" "$OUTDIR/pacs_rangespec_{0}.dat" 16

#SPIRE

#./hload.exe prepare pointing spire 1 "$INDIR/spire/photo_large/myTable_*.txt" "$OUTDIR/spire_large_{0}.dat" 16

#./hload.exe prepare pointing spire 2 "$INDIR/spire/photo_small/myTable_*.txt" "$OUTDIR/spire_small_{0}.dat" 16

#./hload.exe prepare pointing spire 3 "$INDIR/spire/photo_fail/myTable_*.txt" "$OUTDIR/spire_fail_{0}.dat" 16

#./hload.exe prepare pointing spire 3 "$INDIR/spire/parallel/myTable_*.txt" "$OUTDIR/spire_parallel_{0}.dat" 16

#./hload.exe prepare pointing spire 4 "$INDIR/spire/spectro1/myTable_*.txt" "$OUTDIR/spire_spectro1_{0}.dat" 16

#./hload.exe prepare pointing spire 8 "$INDIR/spire/spectro7/myTable_*.txt" "$OUTDIR/spire_spectro7_{0}.dat" 16

./hload.exe prepare pointing spire 16 "$INDIR/spire/spectro64/myTable_*.txt" "$OUTDIR/spire_spectro64_{0}.dat" 16

#HIFI

#./hload.exe prepare pointing hifi 1 "$INDIR/hifi/point/myTable_*.txt" "$OUTDIR/hifi_pointing_{0}.dat" 16

#./hload.exe prepare pointing hifi 2 "$INDIR/hifi/scan/myTable_*.txt" "$OUTDIR/hifi_scan_{0}.dat" 16

#./hload.exe prepare pointing hifi 4 "$INDIR/hifi/map/myTable_*.txt" "$OUTDIR/hifi_map_{0}.dat" 16
