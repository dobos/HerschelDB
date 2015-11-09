#!/bin/bash

source ./settings.sh

INDIR="$HERSCHEL_TEMP/pointing"

../bin/hload load pointing "$INDIR/pacs_*.dat" 16
../bin/hload load pointing "$INDIR/spire_*.dat" 16
../bin/hload load pointing "$INDIR/hifi_*.dat" 16