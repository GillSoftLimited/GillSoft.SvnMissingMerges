using SharpSvn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GillSoft.SvnMissingMerges
{
    public interface IResultWriter
    {
        void WriteResults(CommandLineParameters commandLineParameters, List<SvnMergesEligibleEventArgs> missingRevisions);

        void End();

    }
}
