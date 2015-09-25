#!/bin/bash

INDIR="//blackhole/data/Raid6_2/vo/Herschel/Raw"
OUTDIR="../obs"

#PACS

../bin/hload prepare obs pacs "$INDIR/obs/pacs_photo.txt" "$OUTDIR/pacs_photo.dat"
../bin/hload prepare obs pacs "$INDIR/obs/pacs_parallel.txt" "$OUTDIR/pacs_parallel.dat"
../bin/hload prepare obs pacs "$INDIR/obs/pacs_spectro_range.txt" "$OUTDIR/pacs_spectro_range.dat"
../bin/hload prepare obs pacs "$INDIR/obs/pacs_spectro_line.txt" "$OUTDIR/pacs_spectro_line.dat"

../bin/hload prepare obs spire "$INDIR/obs/spire_photo.txt" "$OUTDIR/spire_photo.dat"
# use PACS file
# ../bin/hload prepare obs spire "$INDIR/obs/spire_parallel.txt" "$OUTDIR/spire_parallel.dat"
../bin/hload prepare obs spire "$INDIR/obs/spire_spectro.txt" "$OUTDIR/spire_spectro.dat"

#../bin/hload prepare obs hifi "$INDIR/Hifi/hifi_all.headerF" "$OUTDIR/hifi.dat"

