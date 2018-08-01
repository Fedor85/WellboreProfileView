using System.Collections.Generic;
using System.Windows.Media.Media3D;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WellboreProfileView.Domain.Services;
using WellboreProfileView.Infrastructure;
using WellboreProfileView.Mappers;
using WellboreProfileView.Models;
using WellboreProfileView.Test.Properties;
using WellboreProfileView.ViewModels;

namespace WellboreProfileView.Test
{
    [TestClass]
    public class CalculateTrajectoryTestFixture
    {
        [TestMethod]
        public void CalculateTrajectoryTest()
        {
            AutoMapperInitializer.Initialize();
            List<ProfilePathGridViewModel> profilePathGridViewModels = SerializeHelper.DeSerializeFromContents<List<ProfilePathGridViewModel>>(Resources.ProfilePathGridViewModel_9777_2);
            List<ProfilePathPoint> mapProfilePathPoints = Mapper.Map(profilePathGridViewModels, new List<ProfilePathPoint>());
            List<Point3D> profilePathPointsFromXml = SerializeHelper.DeSerializeFromContents<List<Point3D>>(Resources._3DProfilePathPoints_9777_2);
            CalculationTrajectoryService calculationTrajectoryService = new CalculationTrajectoryService();
            List<Point3D> profilePathPoints = calculationTrajectoryService.Get3DProfilePathPoints(mapProfilePathPoints);
            ProfilePathPointsAreEqual(profilePathPointsFromXml, profilePathPoints);
        }

        private void ProfilePathPointsAreEqual(List<Point3D> profilePathPoints1, List<Point3D> profilePathPoints2)
        {
            Assert.AreEqual(profilePathPoints1.Count, profilePathPoints2.Count);
            for (int i = 0; i < profilePathPoints1.Count; i++)
                Assert.AreEqual(profilePathPoints1[i], profilePathPoints2[i]);
        }
    }
}