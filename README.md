# Footprint processing pipeline and database for the Herschel Space Observatory

(c) 2014-2016 Laszlo Dobos, Erika Varga-Verebelyi, Csaba Kiss

Data from the Herschel Space Observatory is freely available to the public but no uniformly processed
catalogue of the observations has been published so far. To date, the Herschel Science Archive does not
contain the exact sky coverage (footprint) of individual observations and supports search for measurements
based on bounding circles only. Drawing on previous experience in implementing footprint databases, we built
the Herschel Footprint Database and Web Services for the Herschel Space Observatory to provide efficient
search capabilities for typical astronomical queries. The database was designed with the following main
goals in mind: (a) provide a unified data model for meta-data of all instruments and observational modes,
(b) quickly find observations covering a selected object and its neighbourhood, (c) quickly find every
observation in a larger area of the sky, (d) allow for finding solar system objects crossing observation fields.
As a first step, we developed a unified data model of observations of all three Herschel instruments for all
pointing and instrument modes. Then, using telescope pointing information and observational meta-data, 
we compiled a database of footprints. As opposed to methods using pixellation of the sphere, we represent 
sky coverage in an exact geometric form allowing for precise area calculations. For easier handling of 
Herschel observation footprints with rather complex shapes, two algorithms were implemented to reduce the outline.
Furthermore, a new visualisation tool to plot footprints with various spherical projections was developed.
Indexing of the footprints using Hierarchical Triangular Mesh makes it possible to quickly find observations
based on sky coverage, time and meta-data. The database is accessible via a web site http://herschel.vo.elte.hu
and also as a set of REST web service functions, which makes it readily usable from programming environments
such as Python or IDL. The web service allows downloading footprint data in various formats including
Virtual Observatory standards.

The research was supported by the Hungarian Scientific Research Fund OTKA NN 103244 and OTKA NN 114560 grants
and PECS contract 4000109997/13/NL/KML of the ESA. Herschel is an ESA space observatory with science instruments
provided by European-led Principal Investigator consortia and with important participation from NASA.
