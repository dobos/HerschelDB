#!/bin/bash

SERVER="FUTURE1"
DB="Herschel_3"
DIR="../sso"

bcp load.ObsSso in "$DIR/sso.dat" -S$SERVER -d$DB -T -c -t" " -r"0x0A"