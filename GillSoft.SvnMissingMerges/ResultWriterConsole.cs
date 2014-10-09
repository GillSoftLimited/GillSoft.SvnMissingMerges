using SharpSvn;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace GillSoft.SvnMissingMerges
{
    internal class ResultWriterConsole : IResultWriter
    {
        IInputOutputHelper io;

        public ResultWriterConsole(IInputOutputHelper io)
        {
            this.io = io;
        }

        private void WriteRevisionDetails(SvnLogEventArgs revision)
        {
            io.WriteLine("Revision: {0}", revision.Revision);
            io.WriteLine("    Author: {0}", revision.Author);
            io.WriteLine("    Time: {0}", revision.Time.ToString(CultureInfo.CurrentUICulture));
            io.WriteLine("    Changes:");
            if (revision.ChangedPaths != null)
            {
                foreach (var item in revision.ChangedPaths)
                {
                    io.WriteLine("            {0} {1}: {2}", item.NodeKind, item.Action, item.Path);
                }
            }
            io.WriteLine();
        }


        void IResultWriter.WriteResults(CommandLineParameters commandLineParameters, List<SvnLogEventArgs> missingRevisions)
        {
            io.WriteLine();
            io.WriteLine("Source Repository: {0}", commandLineParameters.SourceRepository);
            io.WriteLine("Target Repository: {0}", commandLineParameters.TargetRepository);
            io.WriteLine();
            io.WriteLine();

            var missingRevisionsNumbers = missingRevisions.Select(a => a.Revision).ToList();
            io.WriteLine("Missing Revisions:");
            io.WriteLine(string.Join(", ", missingRevisionsNumbers));
            io.WriteLine();
            io.WriteLine("Missing Revisions (summarised):");
            io.WriteLine(string.Join(", ", Utility.GetIntListAsRanges(missingRevisionsNumbers)));
            io.WriteLine();

            foreach (var rev in missingRevisions)
            {
                WriteRevisionDetails(rev);
            }
        }

        void IResultWriter.End()
        {
        }
    }
}
