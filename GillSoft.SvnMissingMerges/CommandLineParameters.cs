#define USE_TEST_REPO1

using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GillSoft.SvnMissingMerges
{
    public class CommandLineParameters
    {
        [Option('s', "source", Required = true, HelpText = "URI of source branch.")]
        public string SourceRepository { get; set; }

        [Option('t', "target", Required = true, HelpText = "URI of target branch.")]
        public string TargetRepository { get; set; }

        [Option('r', "endrev", Required = false, HelpText = "Revision upto which the merges are to be checked. If not provided HEAD is used.")]
        public long? EndVersion { get; set; }

        [Option('l', "log", Required = false, HelpText = "Type of log to be created. Will log to console if not specified. Allowed values are Console, Xml, Text.")]
        public LogTypes LogType { get; set; }

        [Option('?', "help", HelpText = "Show Help")]
        public bool ShowHelpOnly { get; set; }

        private readonly IInputOutputHelper io;

        public CommandLineParameters(IInputOutputHelper io)
        {
            this.io = io;

            try
            {

                var parser = new Parser(a => { 
                    a.CaseSensitive = false;
                    a.IgnoreUnknownArguments = true;
                });

                if (!parser.ParseArguments(Environment.GetCommandLineArgs(), this))
                {
                    this.ShowHelp();
                    throw new InvalidCommandLineParametersException(ExitCodes.InvalidParameters, string.Empty);
                }

                if (this.ShowHelpOnly)
                {
                    this.ShowHelp();
                    throw new InvalidCommandLineParametersException(ExitCodes.ShowHelp, string.Empty);
                }

#if USE_TEST_REPO
                this.SourceRepository = @"file://ARIA/shared/svntest/repo/testrepo/branches/branch_001";
#endif
                if (string.IsNullOrEmpty(this.SourceRepository))
                    throw new InvalidCommandLineParametersException(ExitCodes.InvalidParameters, "Source Repository URI not secified.");

#if USE_TEST_REPO
                this.TargetRepository = @"file://ARIA/shared/svntest/repo/testrepo/trunk";
#endif
                if (string.IsNullOrEmpty(this.TargetRepository))
                    throw new InvalidCommandLineParametersException(ExitCodes.InvalidParameters, "Target Repository URI not secified.");

                if (this.LogType == LogTypes.None)
                {
                    this.LogType = LogTypes.Console;
                }

                DumpParameters();
            }
            catch (MemberAccessException)
            {
                ShowHelp();
                throw new InvalidCommandLineParametersException(ExitCodes.InvalidParameters, string.Empty);
            }
        }

        private void ShowHelp()
        {
            var help = new HelpText
            {
                AddDashesToOption = true,
            };
            help.AddOptions(this);
            var helpText = help.ToString();
            io.WriteLine(helpText);
        }


        private void DumpParameters()
        {
            io.WriteLine("Parameters: ");
            io.WriteLine("  Source Repository: " + this.SourceRepository);
            io.WriteLine("  Target Repository: " + this.TargetRepository);
            if (this.EndVersion.HasValue)
            {
                io.WriteLine("  End Revision     : " + this.EndVersion);
            }
            else
            {
                io.WriteLine("  End Revision     : " + "HEAD");
            }
            io.WriteLine("  Log Type         : " + this.LogType);
            io.WriteLine();
        }
    }
}
