#!/bin/bash

source ./settings.sh

INDIR="$HERSCHEL_SOURCE/Raw/obs"
OUTDIR="$HERSCHEL_TEMP/obs"

#PACS

../bin/hload prepare obs pacs "$INDIR/pacs_photo.txt" "$OUTDIR/pacs_photo.dat"
../bin/hload prepare obs pacs "$INDIR/pacs_parallel.txt" "$OUTDIR/pacs_parallel.dat"
../bin/hload prepare obs pacs "$INDIR/pacs_spectro_range.txt" "$OUTDIR/pacs_spectro_range.dat"
../bin/hload prepare obs pacs "$INDIR/pacs_spectro_line.txt" "$OUTDIR/pacs_spectro_line.dat"

#SPIRE

../bin/hload prepare obs spire "$INDIR/spire_photo.txt" "$OUTDIR/spire_photo.dat"
../bin/hload prepare obs spire "$INDIR/spire_parallel.txt" "$OUTDIR/spire_parallel.dat"
../bin/hload prepare obs spire "$INDIR/spire_spectro.txt" "$OUTDIR/spire_spectro.dat"

#HIFI

../bin/hload prepare obs hifi "$INDIR/hifi.txt" "$OUTDIR/hifi.dat"
