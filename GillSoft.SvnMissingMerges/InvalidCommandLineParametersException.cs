using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GillSoft.SvnMissingMerges
{
    public class InvalidCommandLineParametersException : Exception
    {
        public ExitCodes ExitCode { get; private set; }
        public InvalidCommandLineParametersException(ExitCodes exitCode, string message)
            : base(message)
        {
            this.ExitCode = exitCode;
        }
    }
}
