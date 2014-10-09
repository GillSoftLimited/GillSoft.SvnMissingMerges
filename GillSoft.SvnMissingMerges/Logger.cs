using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GillSoft.SvnMissingMerges
{
    internal class Logger : ILog
    {
        private readonly IInputOutputHelper io;
        public Logger(IInputOutputHelper io)
        {
            this.io = io;
        }
        void ILog.Error(Exception ex)
        {
#if DEBUG
            io.WriteLine("ERROR:" + ex.ToString());
#else
            io.WriteLine("ERROR:" + ex.Message);
#endif
        }
    }
}
