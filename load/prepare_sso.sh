#!/bin/bash

INDIR="//blackhole/data/Raid6_2/vo/Herschel/Raw"
OUTDIR="../sso"

mkdir -p "$OUTDIR"
rm "$OUTDIR/sso.dat"

tail "$INDIR/quality/sso_pacs.txt" -n+5 | awk '{print "1", $1, "1"}' >> "$OUTDIR/sso.dat"
