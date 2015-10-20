#!/bin/bash

source ./settings.sh

DIR="$HERSCHEL_TEMP/sso"

bcp load.Sso in "$DIR/pacs_2.dat" -S$HERSCHEL_SERVER -d$HERSCHEL_DB -T -c -t"|" -r'\n'
