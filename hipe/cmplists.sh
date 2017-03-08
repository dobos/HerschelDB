#!/bin/bash

. counts.sh

mismatch=1

a="ls -1 $1/* | awk -F'/' '{ print \$NF }' | sort"
b="ls -1 $2/* | awk -F'/' '{ print \$NF }' | sort"

echo Files missing from $1
comm -13 <(eval $a) <(eval $b)
echo

echo Files missing from $2
comm -23 <(eval $a) <(eval $b)
echo

echo Comparing files
echo

for f in `comm -12 <(eval $a) <(eval $b)`
do
	echo Comparing $1/$f to $2/$f
	compare_counts "cat $1/$f" "cat $2/$f"
	print_mismatch "cat $1/$f" "cat $2/$f"
	echo
done
echo