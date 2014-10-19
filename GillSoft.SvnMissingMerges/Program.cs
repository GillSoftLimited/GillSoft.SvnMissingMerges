using SharpSvn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace GillSoft.SvnMissingMerges
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitCode = ExitCodes.Success;

            IInputOutputHelper io = new ConsoleHelper();
            ILog logger = new Logger(io);

            var preambleHelper = new PreambleHelper(io);
            preambleHelper.Show();
            try
            {
                FindMissingRevisions(io);
            }
            catch (InvalidCommandLineParametersException ex)
            {
                exitCode = ex.ExitCode;
                if (exitCode != ExitCodes.ShowHelp)
                {
                    logger.Error(ex);
                }
            }
            catch (UriFormatException ex)
            {
                logger.Error(ex);
                exitCode = ExitCodes.InvalidUri;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                exitCode = ExitCodes.GeneralFailure;
            }
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                io.Wait();
            }
#endif
            Exit(exitCode);
        }

        private static void FindMissingRevisions(IInputOutputHelper io)
        {
            var commandLineParameters = new CommandLineParameters(io);

            var branchFirstRevision = SubversionHelper.GetFirstRevision(io, commandLineParameters.SourceRepository);

            if (!branchFirstRevision.HasValue)
            {
                io.WriteLine("No branch revisions found.");
            }
            else
            {
                io.Write("Getting revision information from  branch...");
                var branchRevisions = SubversionHelper.GetBranchLog(commandLineParameters.SourceRepository, commandLineParameters.EndVersion);
                io.WriteLine("Done.");

                if (branchRevisions.Count == 0)
                {
                    io.WriteLine("No branch revisions found.");
                }
                else
                {
                    branchRevisions.Sort((a, b) => { return decimal.Compare(a.Revision, b.Revision); });

                    var minRevision = branchRevisions.Select(a => a.Revision).Min();

                    var mergedRanges = GetMergedRangesInTargetBranch(io, commandLineParameters.SourceRepository, commandLineParameters.TargetRepository, commandLineParameters.EndVersion);

                    var missingRevisions = branchRevisions.Where(a => !mergedRanges.Any(b => a.Revision >= b.Start && a.Revision <= b.End)).ToList();

                    if (missingRevisions.Count == 0)
                    {
                        io.WriteLine("YAY!!! Nothing amiss!!!");
                    }
                    else
                    {
                        DumpResults(io, commandLineParameters, missingRevisions);
                    }
                }
            }
        }


        private static void DumpResults(IInputOutputHelper io, CommandLineParameters commandLineParameters, List<SvnLogEventArgs> missingRevisions)
        {
            IResultWriter resultWriter = new ResultWriterConsole(io);
            switch (commandLineParameters.LogType)
            {
                case LogTypes.Text:
                    {
                        resultWriter = new ResultWriterText(io);
                        break;
                    }
                case LogTypes.Xml:
                    {
                        resultWriter = new ResultWriterXml(io);
                        break;
                    }
            }

            resultWriter.WriteResults(commandLineParameters, missingRevisions);

            resultWriter.End();
        }

        private static List<RangeItem> GetMergedRangesInTargetBranch(IInputOutputHelper io, string sourceRepository, string targetRepository, long? endVersion)
        {
            io.Write("Getting merged ranges in the target repository...");
            var sourceRepoName = Path.GetFileName(sourceRepository);
            var targetRepoName = Path.GetFileName(targetRepository);
            var res = new List<RangeItem>();

            var client = SubversionHelper.GetSvnClient();

            var target = new Uri(targetRepository);

            var options = new SvnLogArgs
            {
                RetrieveAllProperties = true,
                RetrieveChangedPaths = true,
                Start = new SvnRevision(1),
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

            io.WriteLine("Done.");

            return res;
        }

        private static void Exit(ExitCodes exitCode)
        {
            Environment.Exit((int)exitCode);
        }
    }
}
