#!/bin/bash

source ./settings.sh

INDIR="$HERSCHEL_TEMP/pointing"

../bin/hload load pointing "$INDIR/*.dat" 16