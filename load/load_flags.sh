#!/bin/bash

source ./settings.sh

DIR="$HERSCHEL_TEMP/flags"

bcp load.ObsQuality in "$DIR/quality.dat" -S$HERSCHEL_SERVER -d$HERSCHEL_DB -T -c -t" " -r"0x0A"
bcp load.ObsSso in "$DIR/sso.dat" -S$HERSCHEL_SERVER -d$HERSCHEL_DB -T -c -t" " -r"0x0A"

