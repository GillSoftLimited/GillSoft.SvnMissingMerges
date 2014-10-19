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

            var client = GetSvnClient();

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

        #endregion

        public static List<SvnLogEventArgs> GetMissingRevisions(IInputOutputHelper io, CommandLineParameters commandLineParameters)
        {
            var res = new List<SvnLogEventArgs>();

            var branchFirstRevision = SubversionHelper.GetFirstRevision(io, commandLineParameters.SourceRepository);

            if (!branchFirstRevision.HasValue)
            {
                io.WriteLine("No branch revisions found.");
            }
            else
            {
                var tasks = new List<Task>();

                io.WriteLine("Getting revision information from source branch...");
                var branchRevisions = new List<SvnLogEventArgs>();
                tasks.Add(Task.Factory.StartNew(delegate
                {
                    var list = SubversionHelper.GetBranchLog(commandLineParameters.SourceRepository, commandLineParameters.EndVersion);
                    branchRevisions.AddRange(list);
                }));


                io.WriteLine("Getting merged ranges in the target branch...");
                var mergedRanges = new List<RangeItem>();
                tasks.Add(Task.Factory.StartNew(delegate
                {
                    var list = GetMergedRangesInTargetBranch(io, commandLineParameters.SourceRepository, commandLineParameters.TargetRepository,
                        branchFirstRevision, commandLineParameters.EndVersion);
                    mergedRanges.AddRange(list);
                }));

                io.WriteLine("Please wait...");

                Task.WaitAll(tasks.ToArray());

                var missingRevisions = branchRevisions.Where(a => !mergedRanges.Any(b => a.Revision >= b.Start && a.Revision <= b.End)).ToList();

                res.AddRange(missingRevisions);
            }
            return res;
        }

        private static List<RangeItem> GetMergedRangesInTargetBranch(IInputOutputHelper io, string sourceRepository, string targetRepository, long? startRevision, long? endVersion)
        {
            var sourceRepoName = Path.GetFileName(sourceRepository);
            var targetRepoName = Path.GetFileName(targetRepository);
            var res = new List<RangeItem>();

            var client = SubversionHelper.GetSvnClient();

            var target = new Uri(targetRepository);

            var options = new SvnLogArgs
            {
                RetrieveAllProperties = true,
                RetrieveChangedPaths = true,
                Start = startRevision.HasValue ? new SvnRevision(startRevision.Value) : new SvnRevision(1),
                End = endVersion.HasValue ? new SvnRevision(endVersion.Value) : new SvnRevision(SvnRevisionType.Head),
                StrictNodeHistory = true,
            };

            client.Log(target, options, delegate(object sender, SvnLogEventArgs e)
            {
                if (!e.ChangedPaths.Any(a => a.Path.EndsWith(targetRepoName, StringComparison.CurrentCultureIgnoreCase)))
                    return;

                var client2 = SubversionHelper.GetSvnClient();
                var target2 = new SvnUriTarget(targetRepository, new SvnRevision(e.Revision));

                var mergeInfo = default(SvnAppliedMergeInfo);

                if (!client2.GetAppliedMergeInfo(target2, out mergeInfo))
                    return;

                if (mergeInfo == null)
                    return;

                var mergeInfoItem = mergeInfo.AppliedMerges.Where(a => a.Uri.ToString().EndsWith(sourceRepoName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                if (mergeInfoItem == null)
                    return;

                var mergeRanges = mergeInfoItem.MergeRanges.Select(a => new RangeItem(a.Start, a.End)).ToList();

                res.AddRange(mergeRanges);
            });


            return res;
        }


    }
}
