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
                var commandLineParameters = new CommandLineParameters(io);

                var missingRevisions = SubversionHelper.GetMissingRevisions(io, commandLineParameters);
                if (missingRevisions == null || missingRevisions.Count == 0)
                {
                    io.WriteLine("YAY!!! Nothing amiss!!!");
                }
                else
                {
                    DumpResults(io, commandLineParameters, missingRevisions);
                }
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

        private static void DumpResults(IInputOutputHelper io, CommandLineParameters commandLineParameters, List<SvnMergesEligibleEventArgs> missingRevisions)
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

        private static void Exit(ExitCodes exitCode)
        {
            Environment.Exit((int)exitCode);
        }
    }
}
