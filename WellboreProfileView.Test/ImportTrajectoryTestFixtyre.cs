using System.Collections.Generic;
using Aspose.Cells;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WellboreProfileView.Aspose;
using WellboreProfileView.Infrastructure;
using WellboreProfileView.Mappers;
using WellboreProfileView.Models;
using WellboreProfileView.Test.Properties;

namespace WellboreProfileView.Test
{
    [TestClass]
    public class ImportTrajectoryTestFixtyre
    {
        [TestMethod]
        public void ImportTrajectoryTest()
        {
            AutoMapperInitializer.Initialize();
            ExcelImportService excelImportService = new ExcelImportService();
            Workbook trajectoryXls = TestHelper.GetWorkbook(Resources.TrajectoryWellbore);
            string tempFilePath = FileHelper.GetTempReportFileName(".xls");
            trajectoryXls.Save(tempFilePath);
            List<ProfilePathPoint> profilePathPoints = excelImportService.GetProfilePaths(tempFilePath);
            FileHelper.DeleteFile(tempFilePath);
            List<ProfilePathPoint> profilePathPointsFromXml = SerializeHelper.DeSerializeFromContents<List<ProfilePathPoint>>(Resources.ProfilePathPoints_3472Г_1);
            ProfilePathPointsAreEqual(profilePathPointsFromXml, profilePathPoints);
        }

        private void ProfilePathPointsAreEqual(List<ProfilePathPoint> profilePathPoints1, List<ProfilePathPoint> profilePathPoints2)
        {
            Assert.AreEqual(profilePathPoints1.Count, profilePathPoints2.Count);
            for (int i = 0; i < profilePathPoints1.Count; i++)
                Assert.AreEqual(profilePathPoints1[i], profilePathPoints2[i]);
        }
    }
}