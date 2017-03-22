dbs=2

server[1]="future1"
database[1]="Herschel_2"

server[2]="future1"
database[2]="Herschel_3"

for script in $*; do
	filename=$(basename "$script")
	extension="${filename##*.}"
	filename="${filename%.*}"
	echo TEST: $filename
	printf "%-64s" "${database[1]}"
	printf "%-64s\n" "${database[2]}"
	echo
	for i in $(seq 1 $dbs); do
		s=${server[$i]}
		d=${database[$i]}
		sqlcmd -S $s -d $d -E -i $script -o $i.dat
	done
	diff 1.dat 2.dat -y --left-column 
	echo
	echo ===================================================================================================
	echo
done