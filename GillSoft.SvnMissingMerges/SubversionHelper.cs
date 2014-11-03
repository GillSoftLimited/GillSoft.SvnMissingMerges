using SharpSvn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GillSoft.SvnMissingMerges
{
    internal static class SubversionHelper
    {

        #region Utility methods

        public static SvnClient GetSvnClient()
        {
            var res = new SvnClient();
            return res;
        }

        private static long? GetFirstRevision(IInputOutputHelper io, string repository)
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

        #endregion

        public static List<SvnMergesEligibleEventArgs> GetMissingRevisions(IInputOutputHelper io, CommandLineParameters commandLineParameters)
        {
            var res = new List<SvnMergesEligibleEventArgs>();

            var branchFirstRevision = SubversionHelper.GetFirstRevision(io, commandLineParameters.SourceRepository);

            if (!branchFirstRevision.HasValue)
            {
                io.WriteLine("No branch revisions found.");
            }
            else
            {
                var tasks = new List<Task>();

                io.WriteLine("Getting revision information from source branch...");
                var mergesEligible = new List<SvnMergesEligibleEventArgs>();
                tasks.Add(Task.Factory.StartNew(delegate
                {
                    var client = GetSvnClient();
                    var args = new SvnMergesEligibleArgs
                    {
                        Range = new SvnRevisionRange(new SvnRevision(branchFirstRevision.Value), new SvnRevision(SvnRevisionType.Head)),
                        RetrieveChangedPaths = true,
                    };
                    var list = client.ListMergesEligible(new Uri(commandLineParameters.TargetRepository),
                        new Uri(commandLineParameters.SourceRepository), args, delegate(object sender, SvnMergesEligibleEventArgs e)
                        {
                            e.Detach();
                            mergesEligible.Add(e);
                        });
                }));


                io.WriteLine("Getting merged ranges in the target branch...");
                var mergesMerged = new List<SvnMergesMergedEventArgs>();
                tasks.Add(Task.Factory.StartNew(delegate
                {
                    var client = GetSvnClient();
                    var args = new SvnMergesMergedArgs
                    {
                        Range = new SvnRevisionRange(new SvnRevision(branchFirstRevision.Value), new SvnRevision(SvnRevisionType.Head)),
                        RetrieveChangedPaths = false,
                    };
                    var list = client.ListMergesMerged(new Uri(commandLineParameters.TargetRepository),
                        new Uri(commandLineParameters.SourceRepository), args, delegate(object sender, SvnMergesMergedEventArgs e)
                        {
                            e.Detach();
                            mergesMerged.Add(e);
                        });
                }));

                io.WriteLine("Please wait...");

                Task.WaitAll(tasks.ToArray());

                var missingRevisions = mergesEligible.Where(a => !mergesMerged.Any(b => b.Revision == a.Revision)).ToList();

                res.AddRange(missingRevisions);
            }
            return res;
        }

    }
}
