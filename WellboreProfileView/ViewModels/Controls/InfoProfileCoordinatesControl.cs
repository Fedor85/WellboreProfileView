using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using AutoMapper;
using Microsoft.Practices.Unity;
using Prism;
using Prism.Events;
using WellboreProfileView.Events;
using WellboreProfileView.Interfaces.Services;
using WellboreProfileView.Models;

namespace WellboreProfileView.ViewModels
{
    public class InfoProfileCoordinatesControlViewModel : BaseRegionUserControlViewModel, IActiveAware
    {
        [Dependency]
        public IEventAggregator EventAggregator { get; set; }

        [Dependency]
        public ICalculationTrajectoryService CalculationTrajectoryService { get; set; }

        private WellboresRootGridViewModel currentWellboresRootGridViewModel;

        private string displayText;

        private bool active;

        public event EventHandler IsActiveChanged;

        public bool IsActive
        {
            get
            {
                return active;
            }
            set
            {
                if (active != value)
                {
                    active = value;
                    if (active)
                        Activate();
                    else
                        DeActivate();
                }
            }
        }

        public string DisplayText
        {
            get
            {
                return displayText;
            }
            set
            {
                displayText = value;
                RaisePropertyChanged();
            }
        }

        private void Activate()
        {
            EventAggregator.GetEvent<ChageWellboresRootGridViewModelEvent>().Subscribe(ChageWellboresRootGridViewModel);
            EventAggregator.GetEvent<QueryExistenceWellboresRootGridViewModel>().Publish();
        }

        private void DeActivate()
        {
            EventAggregator.GetEvent<ChageWellboresRootGridViewModelEvent>().Unsubscribe(ChageWellboresRootGridViewModel);
        }

        private void ChageWellboresRootGridViewModel(WellboresRootGridViewModel wellboresRootGridViewModel)
        {
            if (currentWellboresRootGridViewModel != null && currentWellboresRootGridViewModel.Equals(wellboresRootGridViewModel))
                return;

            currentWellboresRootGridViewModel = wellboresRootGridViewModel;
            if (currentWellboresRootGridViewModel != null)
                currentWellboresRootGridViewModel.MainItems.AnyCollectionChanged += MainItemsAnyCollectionChanged;

            FillDisplayText();
        }


        private void MainItemsAnyCollectionChanged()
        {
            FillDisplayText();
        }

        private void FillDisplayText()
        {
            if (currentWellboresRootGridViewModel == null)
            {
                DisplayText = String.Empty;
                return;
            }
            StringBuilder text = new StringBuilder();
            {
                foreach (WellboreGridViewModel wellbore in currentWellboresRootGridViewModel.MainItems)
                {
                    text.AppendFormat("Ствол {0}", wellbore.Name);
                    text.AppendLine();
                    List <ProfilePathPoint> profilePathPoints = Mapper.Map(wellbore.ChildItems.ToList(), new List<ProfilePathPoint>());
                    List<Point3D> point3Ds = CalculationTrajectoryService.Get3DProfilePathPoints(profilePathPoints);
                    foreach (Point3D point3D in point3Ds)
                    {
                        text.AppendFormat("   - точка : x={0:0.00} | y={1:0.00} | z={2:0.00}", point3D.X, point3D.Y, point3D.Z);
                        text.AppendLine();
                    }
                }
            }
            DisplayText = text.ToString();
        }
    }
}