using SharpSvn;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace GillSoft.SvnMissingMerges
{
    internal class ResultWriterText : IResultWriter
    {
        private static string LogFileName = "GillSoft.SvnMissingMerges.Results.txt";

        private readonly IInputOutputHelper io;
        private readonly FileStream fs;
        private readonly StreamWriter sw;
        private readonly string logFilePath ;

        public ResultWriterText(IInputOutputHelper io)
        {
            this.io = io;

            this.logFilePath = Path.GetFullPath(@".\" + LogFileName);

            this.fs = new FileStream(this.logFilePath, FileMode.Create);
            this.sw = new StreamWriter(fs);
            sw.AutoFlush = true;
        }

        private void WriteRevisionDetails(SvnLogEventArgs revision)
        {
            sw.WriteLine("revision: {0}", revision.Revision);
            sw.WriteLine("    Author: {0}", revision.Author);
            sw.WriteLine("    Time: {0}", revision.Time.ToString(CultureInfo.CurrentUICulture));
            sw.WriteLine("    Changes:");
            if (revision.ChangedPaths != null)
            {
                foreach (var item in revision.ChangedPaths)
                {
                    sw.WriteLine("            {0} {1}: {2}", item.NodeKind, item.Action, item.Path);
                }
            }
            sw.WriteLine();
        }


        void IResultWriter.WriteResults(CommandLineParameters commandLineParameters, List<SvnLogEventArgs> missingRevisions)
        {
            sw.WriteLine("Source Repository: {0}", commandLineParameters.SourceRepository);
            sw.WriteLine("Target Repository: {0}", commandLineParameters.TargetRepository);
            sw.WriteLine();
            sw.WriteLine();

            var missingrevisionsNumbers = missingRevisions.Select(a => a.Revision).ToList();
            sw.WriteLine("Missing revisions:");
            sw.WriteLine(string.Join(", ", missingrevisionsNumbers));
            sw.WriteLine();
            sw.WriteLine("Missing revisions (summarised):");
            sw.WriteLine(string.Join(", ", Utility.GetIntListAsRanges(missingrevisionsNumbers)));
            sw.WriteLine();

            foreach (var rev in missingRevisions)
            {
                WriteRevisionDetails(rev);
            }
        }

        void IResultWriter.End()
        {
            sw.Flush();
            fs.Flush();
            fs.Close();
            io.WriteLine("Results written to: " + logFilePath);
#if DEBUG
            Process.Start(logFilePath);
#endif
        }

    }
}
