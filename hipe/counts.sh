#!/bin/bash

function count() {
	eval "$1 | grep -c '[^[:space:]]'"
}

function count_missing() {
	comm -23 <(eval "$1" | sort) <(eval "$2" | sort) | wc -l
}

function count_extra() {
	comm -13 <(eval "$1" | sort) <(eval "$2" | sort) | wc -l
}

function print_mismatch() {
	if [ $mismatch -eq "1" ]; then
		comm -3 <(eval "$1" | sort) <(eval "$2" | sort)
	fi
}

function compare_counts() {
	c1=$(count "$1")
	c2=$(count "$2")
	echo "File A: $c1, File B: $c2"
	if [ $c1 -ne $c2 ]; then
		echo "Numbers don't match!"
	else
		echo "Numbers match"
	fi
	cm=$(count_missing "$1" "$2")
	cs=$(count_extra "$1" "$2")
	echo "Missing: $cm, Extra: $cs"
}