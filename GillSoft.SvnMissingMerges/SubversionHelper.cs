using SharpSvn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace GillSoft.SvnMissingMerges
{
    internal static class SubversionHelper
    {
       
        private static SvnClient GetSvnClient()
        {
            var client = new SvnClient();
            return client;
        }

        public static long? GetFirstRevision(IInputOutputHelper io, string repository)
        {
            var res = default(long?);
            var client = GetSvnClient();

            var target = new Uri(repository);

            var options = new SvnLogArgs
            {
                Start = new SvnRevision(1),
                StrictNodeHistory = true,
                Limit = 1
            };

            var log = default(Collection<SvnLogEventArgs>);

            if (client.GetLog(target, options, out log))
            {
                if (log != null && log.Count > 0)
                {
                    res = log[0].Revision;
                }
            }

            return res;
        }

        public static List<SvnLogEventArgs> GetBranchLog(string repository, long? endVersion)
        {
            var res = new List<SvnLogEventArgs>();

            var client = new SvnClient();

            var target = new Uri(repository);

            var options = new SvnLogArgs
            {
                RetrieveMergedRevisions = true,
                RetrieveAllProperties = true,
                RetrieveChangedPaths = true,
                Start = new SvnRevision(1),
                End = endVersion.HasValue ? new SvnRevision(endVersion.Value) : new SvnRevision(SvnRevisionType.Head),
                StrictNodeHistory = true,
            };

            Collection<SvnLogEventArgs> log = null;

            if (client.GetLog(target, options, out log))
            {
                if (log != null)
                {
                    res.AddRange(log);
                }
            }

            return res;
        }

    }
}
