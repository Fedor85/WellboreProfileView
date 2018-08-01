using System;
using System.Collections.Generic;
using Aspose.Cells;
using Utils;
using WellboreProfileView.Interfaces.Services;
using WellboreProfileView.Models;
using WellboreProfileViewAspose.Aspos;

namespace WellboreProfileView.Aspose
{
    public class ExcelImportService : IImportService
    {
        public ExcelImportService()
        {
            Initializer.SetupAsposeLicense();
        }

        public List<ProfilePathPoint> GetProfilePaths(string filePath)
        {
            Workbook workbook = new Workbook(filePath);
            Worksheet activeWorksheet = workbook.Worksheets[workbook.Worksheets.ActiveSheetIndex];
            Range range = AsposeHelper.GetRange(activeWorksheet, RangeNames.ProfileImportData);
            if (range == null)
                throw new RangeNotFoundException(String.Format("Не найдена область {0}", RangeNames.ProfileImportData));

            return GetProfilePaths(range);
        }


        private List<ProfilePathPoint> GetProfilePaths(Range range)
        {
            List<ProfilePathPoint> profilePaths = new List<ProfilePathPoint>();
            for (int rowIndex = range.FirstRow; rowIndex < range.RowCount + range.FirstRow; rowIndex++)
            {
                profilePaths.Add(GetProfilePath(range, rowIndex));
            }

            return profilePaths;
        }

        private ProfilePathPoint GetProfilePath(Range range, int rowIndex)
        {
            ProfilePathPoint profilePathPoint = new ProfilePathPoint();
            profilePathPoint.VerticalDepth = ConvertParser.GetConvertValue<double>(range.Worksheet.Cells[rowIndex, range.FirstColumn].StringValue);
            profilePathPoint.InclinationAngle = ConvertParser.GetConvertValue<double>(range.Worksheet.Cells[rowIndex, range.FirstColumn + 1].StringValue);
            profilePathPoint.AzimuthAngle = ConvertParser.GetConvertValue<double>(range.Worksheet.Cells[rowIndex, range.FirstColumn + 2].StringValue);
            profilePathPoint.Extension = ConvertParser.GetConvertValue<double>(range.Worksheet.Cells[rowIndex, range.FirstColumn + 3].StringValue);
            return profilePathPoint;
        }
    }
}