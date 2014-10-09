using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GillSoft.SvnMissingMerges
{
    public enum LogTypes
    {
        None,
        Console,
        Xml,
        Text,
    }

    public enum ExitCodes
    {
        Success,
        GeneralFailure,
        ShowHelp,
        InvalidParameters,
        InvalidUri
    }
}
