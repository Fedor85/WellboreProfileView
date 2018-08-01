using System;
using System.Collections.Generic;
using Aspose.Cells;

namespace WellboreProfileView.Aspose
{
    public static class AsposeHelper
    {
        public static Range GetRange(Worksheet worksheet, string nameRange)
        {
            foreach (Range range in GetRanges(worksheet.Workbook, nameRange))
            {
                if (range.Worksheet == worksheet)
                    return range;
            }
            return null;
        }

        public static List<Range> GetRanges(Workbook workbook, string rangeName)
        {
            List<Range> ranges = new List<Range>();
            foreach (Name name in GetName(workbook, rangeName))
                ranges.Add(name.GetRange());

            return ranges;
        }

        public static List<Name> GetName(Workbook workbook, string text)
        {
            List<Name> names = new List<Name>();
            foreach (Name name in workbook.Worksheets.Names)
            {
                if (name.Text.Contains(text))
                    names.Add(name);
            }
            return names;
        }
    }
}