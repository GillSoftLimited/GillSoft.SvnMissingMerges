#GillSoft.SvnMissingMerges#
=========================

Utility (written in C#) to find revisions missed during SVN merge operations.

####Dependencies:####

1. SharpSVN
2. CommandLineParser

Both dependencies are resolved using Nuget. Just clone the repository and build.

####Usage####
-s, --source    Required. URI of source branch.

-t, --target    Required. URI of target branch.

-r, --endrev    Revision upto which the merges are to be checked. If not provided HEAD is used.

-l, --log       Type of log to be created. Will log to console if not specified. Allowed values are Console, Xml, Text.

-?, --help      Show Help
