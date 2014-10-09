using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GillSoft.SvnMissingMerges
{
    public interface IInputOutputHelper
    {
        void Wait();
        void WriteLine(string value, params object[] args);
        void WriteLine();
    }
}
