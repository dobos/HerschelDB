#!/bin/bash

source ./settings.sh

DIR="$HERSCHEL_TEMP/pointing"

bcp load.HifiPointing in "$DIR/hifi_pointing.dat" -S$HERSCHEL_SERVER -d$HERSCHEL_DB -T -c -t" " -r"0x0A"
bcp load.HifiAngle in "$DIR/hifi_angle.dat" -S$HERSCHEL_SERVER -d$HERSCHEL_DB -T -c -t"\t" -r"0x0A"