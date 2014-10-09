using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GillSoft.SvnMissingMerges
{
    internal static class Utility
    {
        public static List<RangeItem> GetIntListAsRanges(List<long> values)
        {
            var res = new List<RangeItem>();

            if (values != null && values.Count > 0)
            {
                values.Sort();
                var ranges = numListToPossiblyDegenerateRanges(values).Select(a => new RangeItem(a.Item1, a.Item2)).ToList();
                res.AddRange(ranges);
            }
            return res;
        }

        private static IEnumerable<Tuple<long, long>> numListToPossiblyDegenerateRanges(IEnumerable<long> numList)
        {
            var currentRange = default(Tuple<long, long>);
            foreach (var num in numList)
            {
                if (currentRange == null)
                {
                    currentRange = Tuple.Create(num, num);
                }
                else if (currentRange.Item2 == num - 1)
                {
                    currentRange = Tuple.Create(currentRange.Item1, num);
                }
                else
                {
                    yield return currentRange;
                    currentRange = Tuple.Create(num, num);
                }
            }
            if (currentRange != null)
            {
                yield return currentRange;
            }
        }
    }
}
