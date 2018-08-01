using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;
using WellboreProfileView.Models;

namespace WellboreProfileView.Interfaces.Services
{
    public interface ICalculationTrajectoryService
    {
        List<Point3D> Get3DProfilePathPoints(List<ProfilePathPoint> profilePathPoints);

        List<Point> GetHorizontalProjectionProfilePathPoints(List<ProfilePathPoint> profilePathPoints);

        List<Point> GetVerticalProjectionProfilePathPoints(List<ProfilePathPoint> profilePathPoints);
    }
}