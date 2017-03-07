# Download

## HIFI

*list/hifi.txt*
List of all HIFI observation IDs

*hifi_header.py*
Download all HIFI observation headers and store under obs/hifi.txt

*hifi_pointing.py*
Download HIFI extended headers that contain beam positions for H/V and store in pointing/hifi/hifi.txt
Output file will require further processing

## PACS photo

*list/pacs_photo.txt*
Contains PACS photo observation without parallel
Contains 20237 rows

*pacs_photo_header.py*
Download all PACS photo headers and store in obs/pacs_photo.txt

*pacs_photo_pointing.py*
Download all PACS photo pointing files and store int pointing/pacs/photo/MyTable_[obsid].txt

## PACS line spectro

*list/pacs_spectro_line.txt*
List of PACS line spectro observation IDs
Contains 2840 rows

*pacs_spectro_line_header.py*
Download PACS line spectro headers and store in obs/pacs_spectro_line.txt

*pacs_spectro_line_pointing.py*
Download PACS line spectro pointings and stores in pointing/pacs/spectro_line/MyTable_[obsid].txt

## PACS range spectro - MISSING

*list/pacs_spectro_range.txt*
List of PACS range spectro observation IDs
Contains 3316 rows

*pacs_spectro_range_header.py*
MISSING

*pacs_spectro_range_pointing.py*
MISSING

## SPIRE photo

*list/spire_photo.txt*
List of SPIRE photo observations IDs without parallel
Contains 6593 rows

*spire_photo_header.py*
Download SPIRE photo headers and store in obs/spire_photo.txt

*spire_photo_pointing.py*
Download SPIRE photo pointing files and store in pointing/spire/photo/myTable_[obsid].txt

## SPIRE spectro

*list/spire_spectro.txt*
List of all SPIRE spectro observations IDs
Contains 2175 rows

*list/spire_spectro_nopoint.txt*
List of SPIRE spectro observations without pointing information
These are all calibrations and won't be used
Contains 24 rows

*list/spire_spectro_pointed.txt*
List of single point SPIRE spectro observations
Contains 2129 rows

*list/spire_spectro_raster.txt*
List of raster SPIRE specro observations
Contains 22 rows

*spire_spectro_header.py*
Download all SPIRE spectro observations headers and store in obs/spire_spectro.txt

*spire_spectro_pointed_pointing.py*
Download all SPIRE pointed spectro observation pointing files and store in pointing/spire/spectro/myTable_[obsid].txt

*spire_spectro_raster_pointing.py
Download all SPIRE raster spectro observation pointing files and store in pointing/spire/spectro/myTable_[obsid].txt


## Parallel

*list/parallel.txt*
List of PACS/SPIRE parallel observation IDs
Contains 856 rows

*pacs_parallel_header.py*
Downloads PACS/SPIRE parallel photo obsevation headers and stores in obs/pacs_parallel.txt

*spire_parallel_header.py* -- MISSING



# Output files

*obs/hifi.txt* -- OK

*obs/pacs_parallel.txt* -- OK

*obs/pacs_photo.txt* -- OK

*obs/pacs_spectro_line.txt*

*obs/pacs_spectro_range.txt* -- MISSING

*obs/spire_parallel.txt* -- MISSING

*obs/spire_photo.txt*

*obs/spire_spectro.txt*

*pointing/hifi/hifi.txt

*pointing/pacs/photo/myTable_[obsid].txt*

*pointing/pacs/specro_line/myTable_[obsid].txt*

*pointing/pacs/specro_range/myTable_[obsid].txt*

*pointing/spire/spectro_pointed/myTable_[obsid].txt*

*pointing/spire/spectro_raster/myTable_[obsid].txt*

# Transform

# Load