using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;
using AutoMapper;
using Microsoft.Practices.Unity;
using Petzold.Media3D;
using Prism;
using Prism.Events;
using Utils;
using WellboreProfileView.Enums;
using WellboreProfileView.Events;
using WellboreProfileView.Interfaces.Services;
using WellboreProfileView.Models;

namespace WellboreProfileView.ViewModels
{
    public class DrawRange3DControlViewModel : BaseRegionUserControlViewModel, IActiveAware
    {
        [Dependency]
        public IEventAggregator EventAggregator { get; set; }

        [Dependency]
        public ICalculationTrajectoryService CalculationTrajectoryService { get; set; }

        private FeedbackViewport3D drawingRange3D;

        private TransformMatrix transformMatrix;

        private DrawSetting drawSetting;

        private WellboresRootGridViewModel currentWellboresRootGridViewModel;

        private Dictionary<long, List<Point3D>> wellboreProfilePoints;

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

        public FeedbackViewport3D DrawingRange3D
        {
            get
            {
                return drawingRange3D;
            }
            set
            {
                bool beforeNull = drawingRange3D == null;
                drawingRange3D = value;
                RaisePropertyChanged();
                if (beforeNull && value != null)
                {
                    transformMatrix = new TransformMatrix(drawingRange3D);
                    ReDraw();
                }
            }
        }

        public DrawRange3DControlViewModel()
        {
            drawSetting = new DrawSetting();
            wellboreProfilePoints = new Dictionary<long, List<Point3D>>();
        }

        private void Activate()
        {
            EventAggregator.GetEvent<ChageWellboresRootGridViewModelEvent>().Subscribe(ChageWellboresRootGridViewModel);
            EventAggregator.GetEvent<QueryExistenceWellboresRootGridViewModel>().Publish();
        }

        private void DeActivate()
        {
            if (DrawingRange3D != null)
                DrawingRange3D.Children.Clear();

            EventAggregator.GetEvent<ChageWellboresRootGridViewModelEvent>().Unsubscribe(ChageWellboresRootGridViewModel);
        }

        private void ChageWellboresRootGridViewModel(WellboresRootGridViewModel wellboresRootGridViewModel)
        {
            if (currentWellboresRootGridViewModel != null && currentWellboresRootGridViewModel.Equals(wellboresRootGridViewModel))
                return;

            currentWellboresRootGridViewModel = wellboresRootGridViewModel;
            if (currentWellboresRootGridViewModel != null)
                currentWellboresRootGridViewModel.MainItems.AnyCollectionChanged += MainItemsAnyCollectionChanged;

            RefreshData();
            ReDraw();
        }

        private void MainItemsAnyCollectionChanged()
        {
            RefreshData();
            ReDraw();
        }

        private void RefreshData()
        {
            if (DrawingRange3D != null)
                DrawingRange3D.Children.Clear();

            wellboreProfilePoints.Clear();
            foreach (WellboreGridViewModel wellbore in currentWellboresRootGridViewModel.MainItems)
            {
                List<ProfilePathPoint> profilePathPoints = Mapper.Map(wellbore.ChildItems.ToList(), new List<ProfilePathPoint>());
                wellboreProfilePoints.Add(wellbore.GetHashCode(), CalculationTrajectoryService.Get3DProfilePathPoints(profilePathPoints));
            }
        }

        private void ReDraw()
        {
            if (DrawingRange3D == null)
                return;

            List<Point3D> allProfilePoints = new List<Point3D>();
            foreach (KeyValuePair<long, List<Point3D>> profilePoints in wellboreProfilePoints)
            {
                if (profilePoints.Value.Count < 2)
                    continue;

                allProfilePoints.AddRange(profilePoints.Value);
            }

            if (allProfilePoints.Count < 2)
                return;

            Size3D profileSize3D = GetPointsSize3D(allProfilePoints);
            Rect3D profileRect3D = GetPointsRect3D(profileSize3D, allProfilePoints);
            Size3D marginProfileSize3D = GetMarginProfileSize(profileSize3D, drawSetting);
            Size3D coubeSize3D = GetCubeSize3D(marginProfileSize3D);
            Rect3D coube3D = GetСenteredRect3D(coubeSize3D);
            Point3D offset = GetOffsetXYZ(coube3D, profileRect3D);
            Point3D upPoint3D = GetUpPoint3D(allProfilePoints);
            Point3D basePoint = GetOffsetPoint3D(upPoint3D, offset);
            CoordinatePlaneType coordinatePlaneType = GetCoordinatePlaneType(GetOffsetPoint3D(GetBottomPoint3D(allProfilePoints), offset));

            ProfileModelVisual3DGroup profileModelVisual3DGroup = new ProfileModelVisual3DGroup();
            profileModelVisual3DGroup.AddLight(new AmbientLight(drawSetting.LightColor));

            AddGridModel3DGroup(profileModelVisual3DGroup, coordinatePlaneType, coube3D, basePoint, drawSetting);
            AddWindChartModel3DGroup(profileModelVisual3DGroup, basePoint, drawSetting);
            AddDrillUnit(profileModelVisual3DGroup.GridModel3DGroup, basePoint, drawSetting);
            foreach (KeyValuePair<long, List<Point3D>> profilePoints in wellboreProfilePoints)
            {
                if (profilePoints.Value.Count < 2)
                    continue;

                AddTrajectoryModel3DGroup(profileModelVisual3DGroup.TrajectoryModel3DGroup, profilePoints.Value, offset, drawSetting);
            }

            profileModelVisual3DGroup.AddTransform3DGroup(GetMainTransform3DGroup(coordinatePlaneType));
            profileModelVisual3DGroup.AddToViewport3D(DrawingRange3D);

            SetCamera(coube3D, drawSetting);
        }

        private void SetCamera(Rect3D coube3D, DrawSetting drawSetting)
        {
            double mediumCoubeSize = coube3D.SizeZ / 2;
            PerspectiveCamera camera = new PerspectiveCamera();
            camera.LookDirection = new Vector3D(0, 0, -1);
            camera.NearPlaneDistance = 1;

            double mediumHorizontalAngleRadian = MathHelper.AngleToRadian(camera.FieldOfView / 2);
            double tanMediumHorizontalAngleRadian = Math.Tan(mediumHorizontalAngleRadian);
            double mediumWidthNearPlaneDistance = tanMediumHorizontalAngleRadian * camera.NearPlaneDistance;
            double zforWidth = mediumCoubeSize * camera.NearPlaneDistance / mediumWidthNearPlaneDistance;

            double mediumDisplayWidth = DrawingRange3D.ActualWidth / 2;
            double mediumDisplayHeight = DrawingRange3D.ActualHeight / 2;
            double displayLength = mediumDisplayWidth / tanMediumHorizontalAngleRadian;
            double tanMediumVerticalAngleRadian = mediumDisplayHeight / displayLength;
            double mediumHeightNearPlaneDistance = tanMediumVerticalAngleRadian * camera.NearPlaneDistance;
            double zforHeight = mediumCoubeSize * camera.NearPlaneDistance / mediumHeightNearPlaneDistance;
            camera.Position = new Point3D(0, 0, (Math.Max(zforWidth, zforHeight) + mediumCoubeSize) * drawSetting.Display3DDistanceFactor);

            DrawingRange3D.Camera = camera;
        }

        private Size3D GetPointsSize3D(List<Point3D> points)
        {
            double minX = points.Min(i => i.X);
            double minY = points.Min(i => i.Y);
            double minZ = points.Min(i => i.Z);
            double maxX = points.Max(i => i.X);
            double maxY = points.Max(i => i.Y);
            double maxZ = points.Max(i => i.Z);
            return new Size3D(Math.Abs(minX - maxX), Math.Abs(minY - maxY), Math.Abs(minZ - maxZ));
        }

        private Rect3D GetPointsRect3D(Size3D profileSize, List<Point3D> points)
        {
            double pointX = points.Min(i => i.X);
            double pointY = points.Max(i => i.Y);
            double pointZ = points.Max(i => i.Z);
            return new Rect3D(new Point3D(pointX, pointY, pointZ), profileSize);
        }

        private Size3D GetMarginProfileSize(Size3D profileSize, DrawSetting drawSetting)
        {
            return new Size3D(profileSize.X + drawSetting.Margins, profileSize.Y + drawSetting.Margins, profileSize.Z + drawSetting.Margins);
        }

        private Size3D GetCubeSize3D(Size3D size3D)
        {
            double maxSize = Math.Max(size3D.X, Math.Max(size3D.Y, size3D.Z));
            return new Size3D(maxSize, maxSize, maxSize);
        }

        private Rect3D GetСenteredRect3D(Size3D marginSize3D)
        {
            Point3D basePoint3D = new Point3D(-marginSize3D.X / 2, marginSize3D.Y / 2, marginSize3D.Z / 2);
            return new Rect3D(basePoint3D, marginSize3D);
        }

        private Point3D GetOffsetXYZ(Rect3D coube3D, Rect3D profileRect3D)
        {
            double offsetX = (profileRect3D.X + profileRect3D.SizeX / 2) * -1;
            double offsetY = profileRect3D.SizeY / 2 - profileRect3D.Y;
            double offsetZ = coube3D.SizeZ / 2;
            return new Point3D(offsetX, offsetY, offsetZ);
        }

        private Point3D GetUpPoint3D(List<Point3D> allProfilePoints)
        {
            return allProfilePoints.Find(point => MathHelper.IsMiscalculationEqual(point.Z, allProfilePoints.Max(item => item.Z)));
        }

        private Point3D GetBottomPoint3D(List<Point3D> allProfilePoints)
        {
            double maxDepth = allProfilePoints.Min(item => item.Z);
            return allProfilePoints.Find(item => MathHelper.IsMiscalculationEqual(item.Z, maxDepth));
        }

        private Point3D GetOffsetPoint3D(Point3D point3D, Point3D offset)
        {
            Point3D newPoint = new Point3D(point3D.X, point3D.Y, point3D.Z);
            newPoint.Offset(offset.X, offset.Y, offset.Z);
            return newPoint;
        }

        private CoordinatePlaneType GetCoordinatePlaneType(Point3D point3D)
        {
            if (point3D.X >= 0 && point3D.Y >= 0)
                return CoordinatePlaneType.I;

            if (point3D.X < 0 && point3D.Y >= 0)
                return CoordinatePlaneType.II;

            if (point3D.X < 0 && point3D.Y < 0)
                return CoordinatePlaneType.III;

            if (point3D.X >= 0 && point3D.Y < 0)
                return CoordinatePlaneType.IV;

            return CoordinatePlaneType.NoN;
        }

        #region Model3DGroup

        #region Grid

        private void AddGridModel3DGroup(ProfileModelVisual3DGroup profileModelVisual3DGroup, CoordinatePlaneType coordinatePlaneType, Rect3D cube3D, Point3D basePoint, DrawSetting drawSetting)
        {
            AddBaseCubeFrame(profileModelVisual3DGroup, basePoint, cube3D, drawSetting, coordinatePlaneType);
            switch (coordinatePlaneType)
            {
                case CoordinatePlaneType.I:
                    AddLeftCubeFrame(profileModelVisual3DGroup.GridModel3DGroup, basePoint, cube3D, drawSetting);
                    AddBottomCubeFrame(profileModelVisual3DGroup.GridModel3DGroup, basePoint, cube3D, drawSetting);
                    break;
                case CoordinatePlaneType.II:
                    AddRightCubeFrame(profileModelVisual3DGroup.GridModel3DGroup, basePoint, cube3D, drawSetting);
                    AddBottomCubeFrame(profileModelVisual3DGroup.GridModel3DGroup, basePoint, cube3D, drawSetting);
                    break;
                case CoordinatePlaneType.III:
                    AddRightCubeFrame(profileModelVisual3DGroup.GridModel3DGroup, basePoint, cube3D, drawSetting);
                    AddUpCubeFrame(profileModelVisual3DGroup.GridModel3DGroup, basePoint, cube3D, drawSetting);
                    break;
                case CoordinatePlaneType.IV:
                    AddUpCubeFrame(profileModelVisual3DGroup.GridModel3DGroup, basePoint, cube3D, drawSetting);
                    AddLeftCubeFrame(profileModelVisual3DGroup.GridModel3DGroup, basePoint, cube3D, drawSetting);
                    break;
            }
        }

        private void AddBaseCubeFrame(ProfileModelVisual3DGroup profileModelVisual3DGroup, Point3D basePoint, Rect3D cube3D, DrawSetting drawSetting, CoordinatePlaneType coordinatePlaneType)
        {
            Point3D point1 = new Point3D(cube3D.X, cube3D.Y, cube3D.Z - cube3D.SizeZ);
            Point3D point2 = new Point3D(point1.X, point1.Y - cube3D.SizeY, point1.Z);
            Point3D point3 = new Point3D(point2.X + cube3D.SizeX, point2.Y, point2.Z);
            Point3D point4 = new Point3D(point3.X, point3.Y + cube3D.SizeY, point3.Z);
            AddLineGeometryModel3D(profileModelVisual3DGroup.GridModel3DGroup, drawSetting.Grid3DMianThickness, drawSetting.GridMainDiffuseMaterial, point1, point2, point3, point4, point1);

            Point3D mainZPoint1 = basePoint;
            Point3D mainZPoint2 = new Point3D(mainZPoint1.X, mainZPoint1.Y, mainZPoint1.Z - cube3D.SizeZ);
            Point3D mainXPoint1 = new Point3D(cube3D.X, mainZPoint2.Y, cube3D.Z - cube3D.SizeZ);
            Point3D mainXPoint2 = new Point3D(mainXPoint1.X + cube3D.SizeZ, mainXPoint1.Y, mainXPoint1.Z);
            Point3D mainYPoint1 = new Point3D(mainZPoint2.X, cube3D.Y, cube3D.Z - cube3D.SizeZ);
            Point3D mainYPoint2 = new Point3D(mainYPoint1.X, cube3D.Y - cube3D.SizeY, mainYPoint1.Z);


            AddLineGeometryModel3D(profileModelVisual3DGroup.GridModel3DGroup, drawSetting.Grid3DMianThickness, drawSetting.GridMainDiffuseMaterial, mainZPoint1, mainZPoint2);
            AddDepthsZ(profileModelVisual3DGroup.TextModelVisual3D, mainZPoint1, mainZPoint2, drawSetting, coordinatePlaneType);

            AddLineGeometryModel3D(profileModelVisual3DGroup.GridModel3DGroup, drawSetting.Grid3DMianThickness, drawSetting.GridMainDiffuseMaterial, mainXPoint1, mainXPoint2);
            AddGridLinesLeftXOffsetY(profileModelVisual3DGroup.GridModel3DGroup, mainXPoint1, mainXPoint2, cube3D.Y - cube3D.SizeY, drawSetting);
            AddGridLinesRightXOffsetY(profileModelVisual3DGroup.GridModel3DGroup, mainXPoint1, mainXPoint2, cube3D.Y, drawSetting);
            AddDepthsX(profileModelVisual3DGroup.TextModelVisual3D, mainXPoint1, mainXPoint2, basePoint, drawSetting);

            AddLineGeometryModel3D(profileModelVisual3DGroup.GridModel3DGroup, drawSetting.Grid3DMianThickness, drawSetting.GridMainDiffuseMaterial, mainYPoint1, mainYPoint2);
            AddGridLinesLeftYOffsetX(profileModelVisual3DGroup.GridModel3DGroup, mainYPoint1, mainYPoint2, cube3D.X, drawSetting);
            AddGridLinesRightYOffsetX(profileModelVisual3DGroup.GridModel3DGroup, mainYPoint1, mainYPoint2, cube3D.X + cube3D.SizeX, drawSetting);
            AddDepthsY(profileModelVisual3DGroup.TextModelVisual3D, mainYPoint1, mainYPoint2, basePoint, drawSetting);
        }

        private void AddLeftCubeFrame(Model3DGroup gridModel3DGroup, Point3D basePoint, Rect3D cube3D, DrawSetting drawSetting)
        {
            //точки рамки
            Point3D point1 = cube3D.Location;
            Point3D point2 = new Point3D(point1.X, point1.Y - cube3D.SizeY, point1.Z);
            Point3D point3 = new Point3D(point2.X, point2.Y, point2.Z - cube3D.SizeZ);
            Point3D point4 = new Point3D(point3.X, point3.Y + cube3D.SizeY, point3.Z);
            //точки проекции основногo перпендикуляра
            Point3D point5 = new Point3D(point1.X, basePoint.Y, point1.Z);
            Point3D point6 = new Point3D(point5.X, point5.Y, point5.Z - cube3D.SizeZ);
            AddLineGeometryModel3D(gridModel3DGroup, drawSetting.Grid3DMianThickness, drawSetting.GridMainDiffuseMaterial, point1, point2, point3, point4, point1);
            AddLineGeometryModel3D(gridModel3DGroup, drawSetting.Grid3DMianThickness, drawSetting.GridMainDiffuseMaterial, point5, point6);

            AddGridLinesRightZOffsetY(gridModel3DGroup, point5, point6, cube3D.Y, drawSetting);
            AddGridLinesLeftZOffsetY(gridModel3DGroup, point5, point6, cube3D.Y - cube3D.SizeY, drawSetting);
            AddGridLinesRightYOffsetZ(gridModel3DGroup, point1, point2, cube3D.Z - cube3D.SizeZ, drawSetting);
        }

        private void AddRightCubeFrame(Model3DGroup gridModel3DGroup, Point3D basePoint, Rect3D cube3D, DrawSetting drawSetting)
        {
            //точки рамки
            Point3D point1 = new Point3D(cube3D.X + cube3D.SizeX, cube3D.Y, cube3D.Z);
            Point3D point2 = new Point3D(point1.X, point1.Y - cube3D.SizeY, point1.Z);
            Point3D point3 = new Point3D(point2.X, point2.Y, point2.Z - cube3D.SizeY);
            Point3D point4 = new Point3D(point3.X, point3.Y + cube3D.SizeY, point3.Z);
            //точки проекции основногo перпендикуляра
            Point3D point5 = new Point3D(point1.X, basePoint.Y, point1.Z);
            Point3D point6 = new Point3D(point5.X, point5.Y, point5.Z - cube3D.SizeZ);
            AddLineGeometryModel3D(gridModel3DGroup, drawSetting.Grid3DMianThickness, drawSetting.GridMainDiffuseMaterial, point1, point2, point3, point4, point1);
            AddLineGeometryModel3D(gridModel3DGroup, drawSetting.Grid3DMianThickness, drawSetting.GridMainDiffuseMaterial, point5, point6);

            AddGridLinesRightZOffsetY(gridModel3DGroup, point5, point6, cube3D.Y, drawSetting);
            AddGridLinesLeftZOffsetY(gridModel3DGroup, point5, point6, cube3D.Y - cube3D.SizeY, drawSetting);
            AddGridLinesRightYOffsetZ(gridModel3DGroup, point1, point2, cube3D.Z - cube3D.SizeZ, drawSetting);
        }

        private void AddUpCubeFrame(Model3DGroup gridModel3DGroup, Point3D basePoint, Rect3D cube3D, DrawSetting drawSetting)
        {
            //точки рамки
            Point3D point1 = new Point3D(cube3D.X, cube3D.Y, cube3D.Z - cube3D.SizeZ);
            Point3D point2 = cube3D.Location;
            Point3D point3 = new Point3D(point2.X + cube3D.SizeX, point2.Y, point2.Z);
            Point3D point4 = new Point3D(point3.X, point3.Y, point3.Z - cube3D.SizeZ);
            //точки проекции основногo перпендикуляра
            Point3D point5 = new Point3D(basePoint.X, point1.Y, point1.Z);
            Point3D point6 = new Point3D(point5.X, point5.Y, point5.Z + cube3D.SizeZ);
            AddLineGeometryModel3D(gridModel3DGroup, drawSetting.Grid3DMianThickness, drawSetting.GridMainDiffuseMaterial, point1, point2, point3, point4, point1);
            AddLineGeometryModel3D(gridModel3DGroup, drawSetting.Grid3DMianThickness, drawSetting.GridMainDiffuseMaterial, point5, point6);

            AddGridLinesLeftZOffsetX(gridModel3DGroup, point5, point6, cube3D.X + cube3D.SizeX, drawSetting);
            AddGridLinesRightZOffsetX(gridModel3DGroup, point5, point6, cube3D.X, drawSetting);
            AddGridLinesLeftXOffsetZ(gridModel3DGroup, point1, point4, cube3D.Z, drawSetting);
        }

        private void AddBottomCubeFrame(Model3DGroup gridModel3DGroup, Point3D basePoint, Rect3D cube3D, DrawSetting drawSetting)
        {
            //точки рамки
            Point3D point1 = new Point3D(cube3D.X, cube3D.Y - cube3D.SizeY, cube3D.Z - cube3D.SizeZ);
            Point3D point2 = new Point3D(point1.X, point1.Y, point1.Z + cube3D.SizeZ);
            Point3D point3 = new Point3D(point2.X + cube3D.SizeX, point2.Y, point2.Z);
            Point3D point4 = new Point3D(point3.X, point3.Y, point3.Z - cube3D.SizeZ);
            //точки проекции основногo перпендикуляра
            Point3D point5 = new Point3D(basePoint.X, point1.Y, point1.Z);
            Point3D point6 = new Point3D(point5.X, point5.Y, point5.Z + cube3D.SizeZ);
            AddLineGeometryModel3D(gridModel3DGroup, drawSetting.Grid3DMianThickness, drawSetting.GridMainDiffuseMaterial, point1, point2, point3, point4, point1);
            AddLineGeometryModel3D(gridModel3DGroup, drawSetting.Grid3DMianThickness, drawSetting.GridMainDiffuseMaterial, point5, point6);

            AddGridLinesLeftZOffsetX(gridModel3DGroup, point5, point6, cube3D.X + cube3D.SizeX, drawSetting);
            AddGridLinesRightZOffsetX(gridModel3DGroup, point5, point6, cube3D.X, drawSetting);
            AddGridLinesLeftXOffsetZ(gridModel3DGroup, point1, point4, cube3D.Z, drawSetting);
        }

        #region GridLines

        //Z

        private void AddGridLinesLeftZOffsetX(Model3DGroup gridModel3DGroup, Point3D basePoint1, Point3D basePoint2, double end, DrawSetting drawSetting)
        {
            AddGridLinesLeftZOffsetX(gridModel3DGroup, basePoint1, basePoint2, end, drawSetting.GrapthStep, drawSetting.Grid3DThickness, drawSetting.GridDiffuseMaterial);
            AddGridLinesLeftZOffsetX(gridModel3DGroup, basePoint1, basePoint2, end, drawSetting.GetCoordinateStep(), drawSetting.Grid3DMianThickness, drawSetting.GridDiffuseMaterial);
        }

        private void AddGridLinesLeftZOffsetX(Model3DGroup gridModel3DGroup, Point3D basePoint1, Point3D basePoint2, double end, double step, double thickness, DiffuseMaterial material)
        {
            for (double i = basePoint1.X; i < end; i += step)
            {
                basePoint1.Offset(step, 0, 0);
                basePoint2.Offset(step, 0, 0);
                if (basePoint1.X > end)
                    break;

                AddLineGeometryModel3D(gridModel3DGroup, thickness, material, basePoint1, basePoint2);
            }
        }

        private void AddGridLinesLeftZOffsetY(Model3DGroup gridModel3DGroup, Point3D basePoint1, Point3D basePoint2, double end, DrawSetting drawSetting)
        {
            AddGridLinesLeftZOffsetY(gridModel3DGroup, basePoint1, basePoint2, end, drawSetting.GrapthStep, drawSetting.Grid3DThickness, drawSetting.GridDiffuseMaterial);
            AddGridLinesLeftZOffsetY(gridModel3DGroup, basePoint1, basePoint2, end, drawSetting.GetCoordinateStep(), drawSetting.Grid3DMianThickness, drawSetting.GridDiffuseMaterial);
        }

        private void AddGridLinesLeftZOffsetY(Model3DGroup gridModel3DGroup, Point3D basePoint1, Point3D basePoint2, double end, double step, double thickness, DiffuseMaterial material)
        {
            for (double i = basePoint1.Y; i > end; i -= step)
            {
                basePoint1.Offset(0, -step, 0);
                basePoint2.Offset(0, -step, 0);
                if (basePoint1.Y < end)
                    break;

                AddLineGeometryModel3D(gridModel3DGroup, thickness, material, basePoint1, basePoint2);
            }
        }

        private void AddGridLinesRightZOffsetX(Model3DGroup gridModel3DGroup, Point3D basePoint1, Point3D basePoint2, double end, DrawSetting drawSetting)
        {
            AddGridLinesRightZOffsetX(gridModel3DGroup, basePoint1, basePoint2, end, drawSetting.GrapthStep, drawSetting.Grid3DThickness, drawSetting.GridDiffuseMaterial);
            AddGridLinesRightZOffsetX(gridModel3DGroup, basePoint1, basePoint2, end, drawSetting.GetCoordinateStep(), drawSetting.Grid3DMianThickness, drawSetting.GridDiffuseMaterial);
        }

        private void AddGridLinesRightZOffsetX(Model3DGroup gridModel3DGroup, Point3D basePoint1, Point3D basePoint2, double end, double step, double thickness, DiffuseMaterial material)
        {
            for (double i = basePoint1.X; i > end; i -= step)
            {
                basePoint1.Offset(-step, 0, 0);
                basePoint2.Offset(-step, 0, 0);
                if (basePoint1.X < end)
                    break;

                AddLineGeometryModel3D(gridModel3DGroup, thickness, material, basePoint1, basePoint2);
            }
        }

        private void AddGridLinesRightZOffsetY(Model3DGroup gridModel3DGroup, Point3D basePoint1, Point3D basePoint2, double end, DrawSetting drawSetting)
        {
            AddGridLinesRightZOffsetY(gridModel3DGroup, basePoint1, basePoint2, end, drawSetting.GrapthStep, drawSetting.Grid3DThickness, drawSetting.GridDiffuseMaterial);
            AddGridLinesRightZOffsetY(gridModel3DGroup, basePoint1, basePoint2, end, drawSetting.GetCoordinateStep(), drawSetting.Grid3DMianThickness, drawSetting.GridDiffuseMaterial);
        }

        private void AddGridLinesRightZOffsetY(Model3DGroup gridModel3DGroup, Point3D basePoint1, Point3D basePoint2, double end, double step, double thickness, DiffuseMaterial material)
        {
            for (double i = basePoint1.Y; i < end; i += step)
            {
                basePoint1.Offset(0, step, 0);
                basePoint2.Offset(0, step, 0);
                if (basePoint1.Y > end)
                    break;

                AddLineGeometryModel3D(gridModel3DGroup, thickness, material, basePoint1, basePoint2);
            }
        }

        //X

        private void AddGridLinesLeftXOffsetY(Model3DGroup gridModel3DGroup, Point3D basePoint1, Point3D basePoint2, double end, DrawSetting drawSetting)
        {
            AddGridLinesLeftXOffsetY(gridModel3DGroup, basePoint1, basePoint2, end, drawSetting.GrapthStep, drawSetting.Grid3DThickness, drawSetting.GridDiffuseMaterial);
            AddGridLinesLeftXOffsetY(gridModel3DGroup, basePoint1, basePoint2, end, drawSetting.GetCoordinateStep(), drawSetting.Grid3DMianThickness, drawSetting.GridDiffuseMaterial);
        }

        private void AddGridLinesLeftXOffsetY(Model3DGroup gridModel3DGroup, Point3D basePoint1, Point3D basePoint2, double end, double step, double thickness, DiffuseMaterial material)
        {
            for (double i = basePoint1.Y; i > end; i -= step)
            {
                basePoint1.Offset(0, -step, 0);
                basePoint2.Offset(0, -step, 0);
                if (basePoint1.Y < end)
                    break;

                AddLineGeometryModel3D(gridModel3DGroup, thickness, material, basePoint1, basePoint2);
            }
        }

        private void AddGridLinesLeftXOffsetZ(Model3DGroup gridModel3DGroup, Point3D basePoint1, Point3D basePoint2, double end, DrawSetting drawSetting)
        {
            AddGridLinesLeftXOffsetZ(gridModel3DGroup, basePoint1, basePoint2, end, drawSetting.GrapthStep, drawSetting.Grid3DThickness, drawSetting.GridDiffuseMaterial);
            AddGridLinesLeftXOffsetZ(gridModel3DGroup, basePoint1, basePoint2, end, drawSetting.GetCoordinateStep(), drawSetting.Grid3DMianThickness, drawSetting.GridDiffuseMaterial);
        }

        private void AddGridLinesLeftXOffsetZ(Model3DGroup gridModel3DGroup, Point3D basePoint1, Point3D basePoint2, double end, double step, double thickness, DiffuseMaterial material)
        {
            for (double i = basePoint1.Z; i < end; i += step)
            {
                basePoint1.Offset(0, 0, step);
                basePoint2.Offset(0, 0, step);
                if (basePoint1.Z > end)
                    break;

                AddLineGeometryModel3D(gridModel3DGroup, thickness, material, basePoint1, basePoint2);
            }
        }

        private void AddGridLinesRightXOffsetY(Model3DGroup gridModel3DGroup, Point3D basePoint1, Point3D basePoint2, double end, DrawSetting drawSetting)
        {
            AddGridLinesRightXOffsetY(gridModel3DGroup, basePoint1, basePoint2, end, drawSetting.GrapthStep, drawSetting.Grid3DThickness, drawSetting.GridDiffuseMaterial);
            AddGridLinesRightXOffsetY(gridModel3DGroup, basePoint1, basePoint2, end, drawSetting.GetCoordinateStep(), drawSetting.Grid3DMianThickness, drawSetting.GridDiffuseMaterial);
        }

        private void AddGridLinesRightXOffsetY(Model3DGroup gridModel3DGroup, Point3D basePoint1, Point3D basePoint2, double end, double step, double thickness, DiffuseMaterial material)
        {
            for (double i = basePoint1.Y; i < end; i += step)
            {
                basePoint1.Offset(0, step, 0);
                basePoint2.Offset(0, step, 0);
                if (basePoint1.Y > end)
                    break;

                AddLineGeometryModel3D(gridModel3DGroup, thickness, material, basePoint1, basePoint2);
            }
        }

        //Y

        private void AddGridLinesLeftYOffsetX(Model3DGroup gridModel3DGroup, Point3D basePoint1, Point3D basePoint2, double end, DrawSetting drawSetting)
        {
            AddGridLinesLeftYOffsetX(gridModel3DGroup, basePoint1, basePoint2, end, drawSetting.GrapthStep, drawSetting.Grid3DThickness, drawSetting.GridDiffuseMaterial);
            AddGridLinesLeftYOffsetX(gridModel3DGroup, basePoint1, basePoint2, end, drawSetting.GetCoordinateStep(), drawSetting.Grid3DMianThickness, drawSetting.GridDiffuseMaterial);
        }

        private void AddGridLinesLeftYOffsetX(Model3DGroup gridModel3DGroup, Point3D basePoint1, Point3D basePoint2, double end, double step, double thickness, DiffuseMaterial material)
        {
            for (double i = basePoint1.X; i > end; i -= step)
            {
                basePoint1.Offset(-step, 0, 0);
                basePoint2.Offset(-step, 0, 0);
                if (basePoint1.X < end)
                    break;

                AddLineGeometryModel3D(gridModel3DGroup, thickness, material, basePoint1, basePoint2);
            }
        }

        private void AddGridLinesRightYOffsetX(Model3DGroup gridModel3DGroup, Point3D basePoint1, Point3D basePoint2, double end, DrawSetting drawSetting)
        {
            AddGridLinesRightYOffsetX(gridModel3DGroup, basePoint1, basePoint2, end, drawSetting.GrapthStep, drawSetting.Grid3DThickness, drawSetting.GridDiffuseMaterial);
            AddGridLinesRightYOffsetX(gridModel3DGroup, basePoint1, basePoint2, end, drawSetting.GetCoordinateStep(), drawSetting.Grid3DMianThickness, drawSetting.GridDiffuseMaterial);
        }

        private void AddGridLinesRightYOffsetX(Model3DGroup gridModel3DGroup, Point3D basePoint1, Point3D basePoint2, double end, double step, double thickness, DiffuseMaterial material)
        {
            for (double i = basePoint1.X; i < end; i += step)
            {
                basePoint1.Offset(step, 0, 0);
                basePoint2.Offset(step, 0, 0);
                if (basePoint1.X > end)
                    break;

                AddLineGeometryModel3D(gridModel3DGroup, thickness, material, basePoint1, basePoint2);
            }
        }

        private void AddGridLinesRightYOffsetZ(Model3DGroup gridModel3DGroup, Point3D basePoint1, Point3D basePoint2, double end, DrawSetting drawSetting)
        {
            AddGridLinesRightYOffsetZ(gridModel3DGroup, basePoint1, basePoint2, end, drawSetting.GrapthStep, drawSetting.Grid3DThickness, drawSetting.GridDiffuseMaterial);
            AddGridLinesRightYOffsetZ(gridModel3DGroup, basePoint1, basePoint2, end, drawSetting.GetCoordinateStep(), drawSetting.Grid3DMianThickness, drawSetting.GridDiffuseMaterial);
        }

        private void AddGridLinesRightYOffsetZ(Model3DGroup gridModel3DGroup, Point3D basePoint1, Point3D basePoint2, double end, double step, double thickness, DiffuseMaterial material)
        {
            for (double i = basePoint1.Z; i > end; i -= step)
            {
                basePoint1.Offset(0, 0, -step);
                basePoint2.Offset(0, 0, -step);
                if (basePoint1.Z < end)
                    break;

                AddLineGeometryModel3D(gridModel3DGroup, thickness, material, basePoint1, basePoint2);
            }
        }

        #endregion

        #endregion

        private void AddWindChartModel3DGroup(ProfileModelVisual3DGroup profileModelVisual3DGroup, Point3D basePoint, DrawSetting drawSetting)
        {
            Point3D centerPoint = new Point3D(basePoint.X, basePoint.Y, basePoint.Z + drawSetting.WindChartUpOffset);
            Point3D north = new Point3D(centerPoint.X, centerPoint.Y + drawSetting.WindChartLengthLine, centerPoint.Z);
            Point3D eash = new Point3D(centerPoint.X + drawSetting.WindChartLengthLine, centerPoint.Y, centerPoint.Z);
            Point3D south = new Point3D(centerPoint.X, centerPoint.Y - drawSetting.WindChartLengthLine, centerPoint.Z);
            Point3D west = new Point3D(centerPoint.X - drawSetting.WindChartLengthLine, centerPoint.Y, centerPoint.Z);

            AddLineGeometryModel3D(profileModelVisual3DGroup.GridModel3DGroup, drawSetting.Grid3DMianThickness, drawSetting.WindChartDiffuseMaterial, centerPoint, north);
            Point3D vertexConeNorth = new Point3D(north.X, north.Y + drawSetting.WindChartLengthCone, north.Z);
            GeometryModel3D coneNorth = GetCone(north, vertexConeNorth, drawSetting.WindChartRadiusBaseCone, drawSetting.WindChartDiffuseMaterial);
            profileModelVisual3DGroup.GridModel3DGroup.Children.Add(coneNorth);
            WireText northName = GetWindChartText(vertexConeNorth, "N");
            northName.HorizontalAlignment = HorizontalAlignment.Center;
            northName.VerticalAlignment = VerticalAlignment.Bottom;
            northName.Color = drawSetting.WindChartDiffuseMaterial.Color;
            profileModelVisual3DGroup.TextModelVisual3D.Add(northName);

            AddLineGeometryModel3D(profileModelVisual3DGroup.GridModel3DGroup, drawSetting.Grid3DMianThickness, drawSetting.WindChartDiffuseMaterial, centerPoint, eash);
            Point3D vertexConeEast = new Point3D(eash.X + drawSetting.WindChartLengthCone, eash.Y, eash.Z);
            GeometryModel3D coneEash = GetCone(eash, vertexConeEast, drawSetting.WindChartRadiusBaseCone, drawSetting.WindChartDiffuseMaterial);
            profileModelVisual3DGroup.GridModel3DGroup.Children.Add(coneEash);
            WireText eastName = GetWindChartText(vertexConeEast, "E");
            eastName.HorizontalAlignment = HorizontalAlignment.Left;
            eastName.VerticalAlignment = VerticalAlignment.Center;
            eastName.Color = drawSetting.WindChartDiffuseMaterial.Color;
            profileModelVisual3DGroup.TextModelVisual3D.Add(eastName);

            AddLineGeometryModel3D(profileModelVisual3DGroup.GridModel3DGroup, drawSetting.Grid3DMianThickness, drawSetting.WindChartDiffuseMaterial, centerPoint, south);
            Point3D vertexConeSouth = new Point3D(south.X, south.Y - drawSetting.WindChartLengthCone, south.Z);
            GeometryModel3D coneSouth = GetCone(south, vertexConeSouth, drawSetting.WindChartRadiusBaseCone, drawSetting.WindChartDiffuseMaterial);
            profileModelVisual3DGroup.GridModel3DGroup.Children.Add(coneSouth);
            WireText southName = GetWindChartText(vertexConeSouth, "S");
            southName.HorizontalAlignment = HorizontalAlignment.Center;
            southName.VerticalAlignment = VerticalAlignment.Top;
            southName.Color = drawSetting.WindChartDiffuseMaterial.Color;
            profileModelVisual3DGroup.TextModelVisual3D.Add(southName);

            AddLineGeometryModel3D(profileModelVisual3DGroup.GridModel3DGroup, drawSetting.Grid3DMianThickness, drawSetting.WindChartDiffuseMaterial, centerPoint, west);
            Point3D vertexConeWest = new Point3D(west.X - drawSetting.WindChartLengthCone, west.Y, west.Z);
            GeometryModel3D coneWest = GetCone(west, vertexConeWest, drawSetting.WindChartRadiusBaseCone, drawSetting.WindChartDiffuseMaterial);
            profileModelVisual3DGroup.GridModel3DGroup.Children.Add(coneWest);
            WireText westName = GetWindChartText(vertexConeWest, "W");
            westName.HorizontalAlignment = HorizontalAlignment.Right;
            westName.VerticalAlignment = VerticalAlignment.Center;
            westName.Color = drawSetting.WindChartDiffuseMaterial.Color;profileModelVisual3DGroup.TextModelVisual3D.Add(westName);
        }

        private void AddDrillUnit(Model3DGroup gridModel3DGroup, Point3D basePoint, DrawSetting drawSetting)
        {
            Point3D baseBottomPoint1 = new Point3D(basePoint.X, basePoint.Y, basePoint.Z + drawSetting.WindChartUpOffset + drawSetting.Grid3DMianThickness);
            Point3D baseBottomPoint2 = new Point3D(baseBottomPoint1.X, baseBottomPoint1.Y, baseBottomPoint1.Z + drawSetting.DrillUnitBaseHeight);
            AddLineGeometryModel3D(gridModel3DGroup, drawSetting.DrillUniThickness, drawSetting.DrillUnitDiffuseMaterial, basePoint, baseBottomPoint1);
            GeometryModel3D baseDrillUnit = GetCylinder(baseBottomPoint1, baseBottomPoint2, drawSetting.DrillUnitBaseRadius, drawSetting.DrillUnitBaseRadius, drawSetting.DrillUnitDiffuseMaterial);
            gridModel3DGroup.Children.Add(baseDrillUnit);

            Point3D upPoint1 = new Point3D(baseBottomPoint2.X, baseBottomPoint2.Y, baseBottomPoint2.Z + drawSetting.DrillUniHeight);
            AddLineGeometryModel3D(gridModel3DGroup, drawSetting.DrillUniThickness, drawSetting.DrillUnitDiffuseMaterial, baseBottomPoint2, upPoint1);

            Point3D upPoint2 = new Point3D(upPoint1.X, upPoint1.Y, upPoint1.Z + drawSetting.DrillUnitBaseHeight);
            GeometryModel3D upDrillUnit = GetCylinder(upPoint1, upPoint2, drawSetting.DrillUnitUpRadius, drawSetting.DrillUnitUpRadius, drawSetting.DrillUnitDiffuseMaterial);
            gridModel3DGroup.Children.Add(upDrillUnit);

            Point3D northBottom = new Point3D(baseBottomPoint2.X, baseBottomPoint2.Y + drawSetting.DrillUnitBaseRadius - drawSetting.DrillUniThickness, baseBottomPoint2.Z);
            Point3D northUp = new Point3D(upPoint1.X, upPoint1.Y + drawSetting.DrillUnitUpRadius - drawSetting.DrillUniThickness, upPoint1.Z);
            AddLineGeometryModel3D(gridModel3DGroup, drawSetting.DrillUniThickness, drawSetting.DrillUnitDiffuseMaterial, northBottom, northUp);

            Point3D eashBottom = new Point3D(baseBottomPoint2.X + drawSetting.DrillUnitBaseRadius - drawSetting.DrillUniThickness, baseBottomPoint2.Y, baseBottomPoint2.Z);
            Point3D eashUp = new Point3D(upPoint1.X + drawSetting.DrillUnitUpRadius - drawSetting.DrillUniThickness, upPoint1.Y, upPoint1.Z);
            AddLineGeometryModel3D(gridModel3DGroup, drawSetting.DrillUniThickness, drawSetting.DrillUnitDiffuseMaterial, eashBottom, eashUp);

            Point3D southBottom = new Point3D(baseBottomPoint2.X, baseBottomPoint2.Y - drawSetting.DrillUnitBaseRadius + drawSetting.DrillUniThickness, baseBottomPoint2.Z);
            Point3D southUp = new Point3D(upPoint1.X, upPoint1.Y - drawSetting.DrillUnitUpRadius + drawSetting.DrillUniThickness, upPoint1.Z);
            AddLineGeometryModel3D(gridModel3DGroup, drawSetting.DrillUniThickness, drawSetting.DrillUnitDiffuseMaterial, southBottom, southUp);

            Point3D westBottom = new Point3D(baseBottomPoint2.X - drawSetting.DrillUnitBaseRadius + drawSetting.DrillUniThickness, baseBottomPoint2.Y, baseBottomPoint2.Z);
            Point3D westUp = new Point3D(upPoint1.X - drawSetting.DrillUnitUpRadius + drawSetting.DrillUniThickness, upPoint1.Y, upPoint1.Z);
            AddLineGeometryModel3D(gridModel3DGroup, drawSetting.DrillUniThickness, drawSetting.DrillUnitDiffuseMaterial, westBottom, westUp);

            Point3D middleNorth = MathHelper.GetMiddleLine(northBottom, northUp);
            Point3D middleEash = MathHelper.GetMiddleLine(eashBottom, eashUp);
            Point3D middleSouth = MathHelper.GetMiddleLine(southBottom, southUp);
            Point3D middleWest = MathHelper.GetMiddleLine(westBottom, westUp);
            AddLineGeometryModel3D(gridModel3DGroup, drawSetting.DrillUniThickness, drawSetting.DrillUnitDiffuseMaterial, northBottom, middleEash, northUp);
            AddLineGeometryModel3D(gridModel3DGroup, drawSetting.DrillUniThickness, drawSetting.DrillUnitDiffuseMaterial, eashBottom, middleSouth, eashUp);
            AddLineGeometryModel3D(gridModel3DGroup, drawSetting.DrillUniThickness, drawSetting.DrillUnitDiffuseMaterial, southBottom, middleWest, southUp);
            AddLineGeometryModel3D(gridModel3DGroup, drawSetting.DrillUniThickness, drawSetting.DrillUnitDiffuseMaterial, westBottom, middleNorth, westUp);
        }

        private void AddTrajectoryModel3DGroup(Model3DGroup trajectoryModel3DGroup, List<Point3D> points, Point3D offset, DrawSetting drawSetting)
        {
            List<Point3D> offsetTrajectoryPoints = new List<Point3D>();
            foreach (Point3D point in points)
                offsetTrajectoryPoints.Add(GetOffsetPoint3D(point, offset));

            AddLineGeometryModel3D(trajectoryModel3DGroup, drawSetting.Trajectory3DThickness, drawSetting.TrajectoryDiffuseMaterial, offsetTrajectoryPoints.ToArray());
        }

        #region Text3D

        private void AddDepthsZ(List<ModelVisual3D> depthsModelVisual3D, Point3D point1, Point3D point2, DrawSetting drawSetting, CoordinatePlaneType coordinatePlaneType)
        {
            int step = drawSetting.GetCoordinateStep();
            int stepCounter = 0;
            for (double i = point1.Z; i > point2.Z; i -= step)
            {
                WireText wire = GetDepthText(point1, stepCounter);
                wire.Transform = GetTextTransform3DGroupZ(point1, coordinatePlaneType);
                wire.Color = drawSetting.GridMainDiffuseMaterial.Color;
                depthsModelVisual3D.Add(wire);
                point1.Offset(0, 0, -step);
                stepCounter -= step;
            }
        }


        private void AddDepthsX(List<ModelVisual3D> depthsModelVisual3D, Point3D point1, Point3D point2, Point3D basePoint3D, DrawSetting drawSetting)
        {
            int step = drawSetting.GetCoordinateStep();
            int stepCounter = 0;
            Point3D basePointX1 = new Point3D(basePoint3D.X, point1.Y, point1.Z + drawSetting.Grid3DMianThickness);
            for (double i = basePointX1.X; i > point1.X; i -= step)
            {
                WireText wire = GetDepthText(basePointX1, -stepCounter);
                wire.Transform = GetTextTransform3DGroupX(basePointX1);
                wire.Color = drawSetting.GridMainDiffuseMaterial.Color;
                depthsModelVisual3D.Add(wire);
                basePointX1.Offset(-step, 0, 0);
                stepCounter += step;
            }
            stepCounter = 0;
            Point3D basePointX2 = new Point3D(basePoint3D.X, point1.Y, point1.Z + drawSetting.Grid3DMianThickness);
            for (double i = basePointX2.X; i < point2.X; i += step)
            {
                WireText wire = GetDepthText(basePointX2, stepCounter);
                wire.Transform = GetTextTransform3DGroupX(basePointX2);
                wire.Color = drawSetting.GridMainDiffuseMaterial.Color;
                depthsModelVisual3D.Add(wire);
                basePointX2.Offset(step, 0, 0);
                stepCounter += step;
            }
        }

        private void AddDepthsY(List<ModelVisual3D> depthsModelVisual3D, Point3D point1, Point3D point2, Point3D basePoint3D, DrawSetting drawSetting)
        {
            int step = drawSetting.GetCoordinateStep();
            int stepCounter = 0;
            Point3D basePointY1 = new Point3D(point1.X, basePoint3D.Y, point1.Z + drawSetting.Grid3DMianThickness);
            for (double i = basePointY1.Y; i < point1.Y; i += step)
            {
                WireText wire = GetDepthText(basePointY1, stepCounter);
                wire.Transform = new Transform3DGroup();
                wire.Color = drawSetting.GridMainDiffuseMaterial.Color;
                depthsModelVisual3D.Add(wire);
                basePointY1.Offset(0, step, 0);
                stepCounter += step;
            }
            stepCounter = step * 2;
            Point3D basePointY2 = new Point3D(point1.X, basePoint3D.Y - step * 2, point1.Z + drawSetting.Grid3DMianThickness);
            for (double i = basePointY2.Y; i > point2.Y; i -= step)
            {
                WireText wire = GetDepthText(basePointY2, -stepCounter);
                wire.Transform = new Transform3DGroup();
                wire.Color = drawSetting.GridMainDiffuseMaterial.Color;
                depthsModelVisual3D.Add(wire);
                basePointY2.Offset(0, -step, 0);
                stepCounter += step;
            }
        }

        private WireText GetDepthText(Point3D point, int depth)
        {
            WireText wire = new WireText();
            wire.Text = String.Format("< {0}", depth);
            wire.Origin = point;
            wire.FontSize = 40;
            wire.Thickness = 2;
            wire.HorizontalAlignment = HorizontalAlignment.Left;
            wire.VerticalAlignment = VerticalAlignment.Center;
            return wire;
        }

        private WireText GetWindChartText(Point3D point, string name)
        {
            WireText wire = new WireText();
            wire.Text = name;
            wire.Origin = point;
            wire.FontSize = 60;
            wire.Thickness = 4;
            wire.Transform = new Transform3DGroup();
            return wire;
        }

        #endregion

        #region Transform3DGroup


        private Transform3DGroup GetMainTransform3DGroup(CoordinatePlaneType coordinatePlaneType)
        {
            Transform3DGroup transformGroup = new Transform3DGroup();

            Vector3D vectorX = new Vector3D(1, 0, 0);
            AxisAngleRotation3D rotationX = new AxisAngleRotation3D(vectorX, -90);
            RotateTransform3D rotateTransformX = new RotateTransform3D(rotationX, new Point3D(0, 0, 0));
            transformGroup.Children.Add(rotateTransformX);

            Vector3D vectorY = new Vector3D(0, 1, 0);
            AxisAngleRotation3D rotationY = new AxisAngleRotation3D(vectorY, GetAngleRotationZ(coordinatePlaneType));
            RotateTransform3D rotateTransformY = new RotateTransform3D(rotationY, new Point3D(0, 0, 0));
            transformGroup.Children.Add(rotateTransformY);

            Vector3D vectorX2 = new Vector3D(1, 0, 0);
            AxisAngleRotation3D rotationX2 = new AxisAngleRotation3D(vectorX2, 15);
            RotateTransform3D rotateTransformX2 = new RotateTransform3D(rotationX2, new Point3D(0, 0, 0));
            transformGroup.Children.Add(rotateTransformX2);

            return transformGroup;
        }

        private int GetAngleRotationZ(CoordinatePlaneType coordinatePlaneType)
        {
            switch (coordinatePlaneType)
            {
                case CoordinatePlaneType.I:
                    return -135;
                case CoordinatePlaneType.II:
                    return 135;
                case CoordinatePlaneType.III:
                    return 45;
                case CoordinatePlaneType.IV:
                    return -45;
                default:
                    return 0;
            }
        }

        private Transform3DGroup GetTextTransform3DGroupZ(Point3D point, CoordinatePlaneType coordinatePlaneType)
        {
            Transform3DGroup transformGroup = new Transform3DGroup();

            Vector3D vectorX = new Vector3D(1, 0, 0);
            AxisAngleRotation3D rotationX = new AxisAngleRotation3D(vectorX, 90);
            RotateTransform3D rotateTransformX = new RotateTransform3D(rotationX, point);
            transformGroup.Children.Add(rotateTransformX);

            Vector3D vectorZ = new Vector3D(0, 0, 1);
            AxisAngleRotation3D rotationZ = new AxisAngleRotation3D(vectorZ, GetAngleRotationTextZ(coordinatePlaneType));
            RotateTransform3D rotateTransformZ = new RotateTransform3D(rotationZ, point);
            transformGroup.Children.Add(rotateTransformZ);

            return transformGroup;
        }

        private int GetAngleRotationTextZ(CoordinatePlaneType coordinatePlaneType)
        {
            switch (coordinatePlaneType)
            {
                case CoordinatePlaneType.I:
                    return 180;
                case CoordinatePlaneType.II:
                    return -90;
                case CoordinatePlaneType.IV:
                    return 90;
                default:
                    return 0;
            }
        }

        private Transform3DGroup GetTextTransform3DGroupX(Point3D point)
        {
            Transform3DGroup transformGroup = new Transform3DGroup();
            Vector3D vectorZ = new Vector3D(0, 0, 1);
            AxisAngleRotation3D rotationZ = new AxisAngleRotation3D(vectorZ, -90);
            RotateTransform3D rotateTransformZ = new RotateTransform3D(rotationZ, point);
            transformGroup.Children.Add(rotateTransformZ);
            return transformGroup;
        }

        #endregion

        #region GeometryModel3D

        private void AddLineGeometryModel3D(Model3DGroup model3DGroup, double thickness, DiffuseMaterial gridMaterial, params Point3D[] point3Ds)
        {
            if (point3Ds.Length < 2)
                return;

            for (int i = 1; i < point3Ds.Length; i++)
            {
                Point3D previousPoint = point3Ds[i - 1];
                Point3D currentPoint = point3Ds[i];
                GeometryModel3D geometryModel3D = GetCylinder(previousPoint, currentPoint, thickness, gridMaterial);
                model3DGroup.Children.Add(geometryModel3D);
            }
        }

        private GeometryModel3D GetCylinder(Point3D point1, Point3D point2, double thickness, DiffuseMaterial diffuseMaterial)
        {
            double radius = thickness / 2;
            return GetCylinder(point1, point2, radius, radius, diffuseMaterial);
        }

        private GeometryModel3D GetCone(Point3D point1, Point3D point2, double radiusBase, DiffuseMaterial diffuseMaterial)
        {
            return GetCylinder(point1, point2, radiusBase, 0, diffuseMaterial);
        }

        private GeometryModel3D GetCylinder(Point3D point1, Point3D point2, double radius1, double radius2, DiffuseMaterial diffuseMaterial)
        {
            Cylinder cylinder = new Cylinder();
            cylinder.Point1 = point1;
            cylinder.Point2 = point2;
            cylinder.Radius1 = radius1;
            cylinder.Radius2 = radius2;
            cylinder.Fold1 = 0.1;
            cylinder.Fold2 = 0.9;
            cylinder.Slices = 36;
            cylinder.Stacks = 1;
            cylinder.EndStacks = 1;

            GeometryModel3D geometryModel3D = new GeometryModel3D();
            geometryModel3D.Geometry = cylinder.Geometry;
            geometryModel3D.Material = diffuseMaterial;
            return geometryModel3D;
        }

        #endregion

        #endregion
    }
}