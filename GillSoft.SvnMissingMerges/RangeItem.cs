using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GillSoft.SvnMissingMerges
{
    class RangeItem
    {
        public long Start { get; private set; }
        public long End { get; private set; }

        public RangeItem(long start, long end)
        {
            this.Start = start;
            this.End = end;
        }

        public override string ToString()
        {
            var res = this.Start == this.End ? this.Start.ToString() : string.Format("{0} - {1}", this.Start, this.End);
            return res;
        }
    }
}
