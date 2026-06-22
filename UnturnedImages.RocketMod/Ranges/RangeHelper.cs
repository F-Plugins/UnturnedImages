using System;
using System.Linq;

namespace UnturnedImages.Ranges
{
    internal static class RangeHelper
    {
        public static Range ParseSingle(string unparsed)
        {
            try
            {
                if (string.IsNullOrEmpty(unparsed))
                {
                    throw new RangeParseException("Given string is empty");
                }

                var parts = unparsed.Split('-').Select(x => x.Trim()).ToArray();

                if (parts.Length > 2)
                {
                    throw new RangeParseException("Too many parts in range");
                }

                var first = ushort.Parse(parts.First());
                var last = ushort.Parse(parts.Last());

                return new Range(first, last);
            }
            catch (Exception ex) when (!(ex is RangeParseException))
            {
                throw new RangeParseException("Could not parse range", ex);
            }
        }

        public static MultiRange ParseMulti(string unparsed)
        {
            try
            {
                if (string.IsNullOrEmpty(unparsed))
                {
                    throw new RangeParseException("Given string is empty");
                }

                var parts = unparsed.Split(',', ';').Select(x => x.Trim()).ToArray();
                var ranges = new Range[parts.Length];

                for (var i = 0; i < parts.Length; i++)
                {
                    ranges[i] = ParseSingle(parts[i]);
                }

                return new MultiRange(ranges);
            }
            catch (Exception ex) when (!(ex is RangeParseException))
            {
                throw new RangeParseException("Could not parse range", ex);
            }
        }
    }
}
