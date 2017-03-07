#!/bin/bash

function count() {
	eval "$1 | grep -c '[^[:space:]]'"
}

function count_missing() {
	comm -23 <(eval "$1" | sort) <(eval "$2" | sort) | wc -l
}

function count_surplus() {
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
	cs=$(count_surplus "$1" "$2")
	echo "Missing: $cm, Surplus: $cs"
}

mismatch="0"

# -------------------------------------------------------------

# HIFI observations
a="cat list/hifi.txt"

echo "Verifying HIFI headers"
b="tail -n +5 obs/hifi.txt | cut -d' ' -f 1"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

echo "Verifying HIFI pointings"
b="tail -n +5 pointing/hifi/hifi.txt | cut -d' ' -f 1"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

-------------------------------------------------------------

PACS photo
a="cat list/pacs_photo.txt"

echo "Verifying PACS photo headers"
b="tail -n +5 obs/pacs_photo.txt | cut -d' ' -f 1"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

echo "Verifying PACS photo pointings"
b="ls -1 pointing/pacs/photo/ | grep -o -E '[0-9]+'"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

# -------------------------------------------------------------

# PACS line spectro
a="cat list/pacs_spectro_line.txt"

echo "Verifying PACS line spectro headers"
b="tail -n +5 obs/pacs_spectro_line.txt | cut -d' ' -f 1"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

echo "Verifying PACS line spectro pointings"
b="ls -1 pointing/pacs/spectro_line/ | grep -o -E '[0-9]+'"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

# -------------------------------------------------------------

# PACS range spectro
a="cat list/pacs_spectro_range.txt"

echo "Verifying PACS range spectro headers"
b="tail -n +5 obs/pacs_spectro_range.txt | cut -d' ' -f 1"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

echo "Verifying PACS range spectro pointings"
b="ls -1 pointing/pacs/spectro_range/ | grep -o -E '[0-9]+'"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

# -------------------------------------------------------------

# SPIRE photo
a="cat list/spire_photo.txt"

echo "Verifying SPIRE photo headers"
b="tail -n +5 obs/spire_photo.txt | cut -d' ' -f 1"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

echo "Verifying SPIRE photo pointings"
b="ls -1 pointing/spire/photo/ | grep -o -E '[0-9]+'"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

# -------------------------------------------------------------

# SPIRE spectro
a="cat list/spire_spectro.txt"

echo "Verifying SPIRE spectro headers"
b="tail -n +5 obs/spire_spectro.txt | cut -d' ' -f 1"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

echo "Verifying SPIRE spectro pointings"
b="ls -1 pointing/spire/spectro | grep -o -E '[0-9]+'"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

echo "Verifying SPIRE spectro pointings of all kinds"
a="{ cat list/spire_spectro_nopoint.txt ; echo ; cat list/spire_spectro_pointed.txt ; echo ; cat list/spire_spectro_raster.txt ; }"
b="ls -1 pointing/spire/spectro | grep -o -E '[0-9]+'"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

# -------------------------------------------------------------

# PARALLEL photo
a="cat list/parallel.txt"

echo "Verifying PACS parallel photo headers"
b="tail -n +5 obs/pacs_parallel.txt | cut -d' ' -f 1"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

echo "Verifying SPIRE parallel photo headers"
b="tail -n +5 obs/spire_parallel.txt | cut -d' ' -f 1"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

echo "Verifying PACS parallel photo pointings"
b="ls -1 pointing/pacs/parallel/ | grep -o -E '[0-9]+'"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

echo "Verifying SPIRE parallel photo pointings"
b="ls -1 pointing/spire/parallel/ | grep -o -E '[0-9]+'"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo
