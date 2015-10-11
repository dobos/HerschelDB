#!/bin/bash

INDIR="//blackhole/data/Raid6_2/vo/Herschel/Raw"
OUTDIR="../quality"

mkdir -p "$OUTDIR"
rm "$OUTDIR/quality.dat"

#tail "$INDIR/quality/quality_pacs.txt"          -n+5 | awk '{print "1", $1, $2=="FAILED"?1:0}' >> "$OUTDIR/quality.dat"
tail "$INDIR/quality/quality_parallel_pacs.txt"  -n+5 | awk '{print "1", $1, $2=="FAILED"?1:0}' >> "$OUTDIR/quality.dat"
tail "$INDIR/quality/quality_spire.txt"          -n+5 | awk '{print "2", $1, $2=="FAILED"?1:0}' >> "$OUTDIR/quality.dat"
tail "$INDIR/quality/quality_parallel_spire.txt" -n+5 | awk '{print "2", $1, $2=="FAILED"?1:0}' >> "$OUTDIR/quality.dat"
tail "$INDIR/quality/quality_parallel_pacs.txt"  -n+5 | awk '{print "4", $1, $2=="FAILED"?1:0}' >> "$OUTDIR/quality.dat"
tail "$INDIR/quality/quality_parallel_spire.txt" -n+5 | awk '{print "4", $1, $2=="FAILED"?1:0}' >> "$OUTDIR/quality.dat"
tail "$INDIR/quality/quality_hifi.txt"           -n+5 | awk '{print "8", $1, $2=="FAILED"?1:0}' >> "$OUTDIR/quality.dat"