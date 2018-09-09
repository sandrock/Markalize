
Markalize file specification
=====================================

Keys and values
-------------------

### Setting keys

TODO: write specifications


### Setting simple values

TODO: write specifications

Simple values are whitespace-trimmed from the left and from the right. Except when:

* a line is not trimmed from the right when it ends with a backslash.


### Setting quoted values 

TODO: write specifications

Simple values are whitespace-trimmed from the left. 

To avoid trimming, you can use the verbatim `@` indicator before the first opening double quote.


Titles for prefixes
-------------------

### Title 1 for first level prefix

TODO: write specifications


### Title 2 for second level prefix

TODO: write specifications


### No more title levels

TODO: explain why

Dimensions
-------------------

TODO: write specifications


Dimension indicators
-------------------

### Using file names

TODO: write specifications

A file name must be composed of:

* a base file name
* a dot
* dot-separated tags
* a dot
* one of these file extensions: txt, md

A tag in a file name have various meanings. They must be composed like described below.

* The `Default` tag value can be used to designate the ultimate fallback file.
* A 5-char culture name can be specified (`fr-FR`, `en-US`, ...)
  * 2 char language name
  * a dash
  * 2 char region name
* A Dimension and value (`T-Tu`, `S-StarTrek`, ...)
  * One or many chars for the dimension
  * a dash
  * One or many chars for the dimension value
* A random string not maching any of the format above


### Using metadata

TODO: write specifications


### Using files from your database

TODO: write specifications


Editing
-------------------

### Hand editing

TODO: write specifications


### Programatic editing

TODO: write specifications


### Future GUIs

TODO: write specifications

