using System.Collections.Generic;
using System.Windows;
using WellboreProfileView.Interfaces;
using WellboreProfileView.Models;

namespace WellboreProfileView.ViewModels
{
    public class DrawProfileControlViewModel : DrawRangeControlViewModel, IDrawProfileControlViewModel
    {
        protected override List<Point> GetPoints(List<ProfilePathPoint> profilePathPoints)
        {
            return CalculationTrajectoryService.GetVerticalProjectionProfilePathPoints(profilePathPoints);
        }
    }
}