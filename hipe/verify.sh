#!/bin/bash

. counts.sh

mismatch="0"

# -------------------------------------------------------------

# HIFI observations
a="cat $1/hifi.txt"

echo "Verifying HIFI headers"
b="tail -n +5 $2/obs/hifi.txt | cut -d' ' -f 1"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

echo "Verifying HIFI pointings"
b="tail -n +5 $2/pointing/hifi/hifi.txt | cut -d' ' -f 1"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

# -------------------------------------------------------------

# PACS photo
a="cat $1/pacs_photo.txt"

echo "Verifying PACS photo headers"
b="tail -n +5 $2/obs/pacs_photo.txt | cut -d' ' -f 1"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

echo "Verifying PACS photo pointings"
b="ls -1 $2/pointing/pacs/photo/ | grep -o -E '[0-9]+'"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

# -------------------------------------------------------------

# PACS line spectro
a="cat $1/pacs_spectro_line.txt"

echo "Verifying PACS line spectro headers"
b="tail -n +5 $2/obs/pacs_spectro_line.txt | cut -d' ' -f 1"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

echo "Verifying PACS line spectro pointings"
b="ls -1 $2/pointing/pacs/spectro_line/ | grep -o -E '[0-9]+'"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

# -------------------------------------------------------------

# PACS range spectro
a="cat $1/pacs_spectro_range.txt"

echo "Verifying PACS range spectro headers"
b="tail -n +5 $2/obs/pacs_spectro_range.txt | cut -d' ' -f 1"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

echo "Verifying PACS range spectro pointings"
b="ls -1 $2/pointing/pacs/spectro_range/ | grep -o -E '[0-9]+'"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

# -------------------------------------------------------------

# SPIRE photo
a="cat $1/spire_photo.txt"

echo "Verifying SPIRE photo headers"
b="tail -n +5 $2/obs/spire_photo.txt | cut -d' ' -f 1"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

echo "Verifying SPIRE photo pointings"
b="ls -1 $2/pointing/spire/photo/ | grep -o -E '[0-9]+'"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

# -------------------------------------------------------------

# SPIRE spectro
a="cat $1/spire_spectro.txt"

echo "Verifying SPIRE spectro headers"
b="tail -n +5 $2/obs/spire_spectro.txt | cut -d' ' -f 1"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

echo "Verifying SPIRE spectro pointings"
b="ls -1 $2/pointing/spire/spectro | grep -o -E '[0-9]+'"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

echo "Verifying SPIRE spectro pointings of all kinds"
a="{ cat $1/spire_spectro_nopoint.txt ; echo ; cat $1/spire_spectro_pointed.txt ; echo ; cat $1/spire_spectro_raster.txt ; }"
b="ls -1 $2/pointing/spire/spectro | grep -o -E '[0-9]+'"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

# -------------------------------------------------------------

# PARALLEL photo
a="cat $1/parallel.txt"

echo "Verifying PACS parallel photo headers"
b="tail -n +5 $2/obs/pacs_parallel.txt | cut -d' ' -f 1"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

echo "Verifying SPIRE parallel photo headers"
b="tail -n +5 $2/obs/spire_parallel.txt | cut -d' ' -f 1"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

echo "Verifying PACS parallel photo pointings"
b="ls -1 $2/pointing/pacs/parallel/ | grep -o -E '[0-9]+'"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo

echo "Verifying SPIRE parallel photo pointings"
b="ls -1 $2/pointing/spire/parallel/ | grep -o -E '[0-9]+'"
compare_counts "$a" "$b"
print_mismatch "$a" "$b"
echo
