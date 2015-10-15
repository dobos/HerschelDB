#!/bin/bash

INDIR="//blackhole/data/Raid6_2/vo/Herschel/Raw"
OUTDIR="../sso"

mkdir -p "$OUTDIR"
rm "$OUTDIR/sso.dat"

../bin/hload.exe prepare sso pacs "$INDIR/sso/herschel-2.out" "$OUTDIR/pacs_2.dat"
