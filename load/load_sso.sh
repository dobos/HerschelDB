#!/bin/bash

SERVER="FUTURE1"
DB="Herschel_3"
DIR="../sso"

bcp load.Sso in "$DIR/pacs_2.dat" -S$SERVER -d$DB -T -c -t"|" -r'\n'
