#!/bin/bash

source ./settings.sh

INDIR="$HERSCHEL_SOURCE/Raw"
OUTDIR="$HERSCHEL_TEMP/sso"

mkdir -p "$OUTDIR"
rm "$OUTDIR/sso.dat"

../bin/hload.exe prepare sso pacs "$INDIR/sso/herschel-2.out" "$OUTDIR/pacs_2.dat"
