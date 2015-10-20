#!/bin/bash

source ./settings.sh

INDIR="$HERSCHEL_TEMP/obs"

../bin/hload load obs "$INDIR/*.dat" 12
