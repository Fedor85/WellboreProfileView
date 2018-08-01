using System.Collections.Generic;
using System.Windows;
using WellboreProfileView.Interfaces;
using WellboreProfileView.Models;

namespace WellboreProfileView.ViewModels
{
    public class DrawPlanControlViewModel : DrawRangeControlViewModel, IDrawPlanControlViewModel
    {
        protected override List<Point> GetPoints(List<ProfilePathPoint> profilePathPoints)
        {
            return CalculationTrajectoryService.GetHorizontalProjectionProfilePathPoints(profilePathPoints);
        }
    }
}