using System.IO;

using Aspose.Cells;

namespace WellboreProfileView.Test
{
    public static class TestHelper
    {
        public static Workbook GetWorkbook(byte[] xls)
        {
            using (MemoryStream memXls = new MemoryStream(xls))
            {
                return new Workbook(memXls);
            }
        }
    }
}