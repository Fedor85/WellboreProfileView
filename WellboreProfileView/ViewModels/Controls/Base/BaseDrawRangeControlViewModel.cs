using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AutoMapper;
using Microsoft.Practices.Unity;
using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using WellboreProfileView.Events;
using WellboreProfileView.Interfaces.Services;
using WellboreProfileView.Models;

namespace WellboreProfileView.ViewModels
{
    public abstract class DrawRangeControlViewModel : BindableBase, IActiveAware
    {
        [Dependency]
        public IEventAggregator EventAggregator { get; set; }

        [Dependency]
        protected ICalculationTrajectoryService CalculationTrajectoryService { get; set; }

        private WellboresRootGridViewModel currentWellboresRootGridViewModel;

        private Dictionary<long, List<Point>> wellboreProfilePoints;

        private DrawSetting DrawSetting { get; set; }

        private Canvas drawingRange;

        private bool active;

        public event EventHandler IsActiveChanged;

        public Canvas DrawingRange
        {
            get
            {
                return drawingRange;
            }
            set
            {
                bool beforeNull = drawingRange == null;
                drawingRange = value;
                RaisePropertyChanged();
                if (beforeNull && value != null)
                    ReDraw();
            }
        }

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

        public ICommand CanvasSizeChangedDCommand { get; set; }

        public DrawRangeControlViewModel()
        {
            DrawSetting = new DrawSetting();
            CanvasSizeChangedDCommand = new DelegateCommand(CanvasSizeChanged);
            wellboreProfilePoints = new Dictionary<long, List<Point>>();
        }
        private void Activate()
        {
            EventAggregator.GetEvent<ChageWellboresRootGridViewModelEvent>().Subscribe(ChageWellboresRootGridViewModel);
            EventAggregator.GetEvent<QueryExistenceWellboresRootGridViewModel>().Publish();
        }

        private void DeActivate()
        {
            if (DrawingRange != null)
                RemoveAllChildren();

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
            wellboreProfilePoints.Clear();
            foreach (WellboreGridViewModel wellbore in currentWellboresRootGridViewModel.MainItems)
            {
                List<ProfilePathPoint> profilePathPoints = Mapper.Map(wellbore.ChildItems.ToList(), new List<ProfilePathPoint>());
                List<Point> points = GetPoints(profilePathPoints);
                wellboreProfilePoints.Add(wellbore.GetHashCode(), GetRevertYPoints(points));
            }
        }

        private List<Point> GetRevertYPoints(List<Point> points)
        {
            List<Point> revertYPoints = new List<Point>();
            foreach (Point point in points)
                revertYPoints.Add(new Point(point.X, -point.Y));

            return revertYPoints;
        }

        protected abstract List<Point> GetPoints(List<ProfilePathPoint> profilePathPoints);

        private void CanvasSizeChanged()
        {
            ReDraw();
        }

        private void ReDraw()
        {
            if (DrawingRange == null)
                return;

            RemoveAllChildren();
            Size drawSize = new Size((int)DrawingRange.ActualWidth, (int)DrawingRange.ActualHeight);
            if (drawSize.Width == 0 || drawSize.Height == 0)
            {
                DrawSetting.Reset();
                return;
            }
            List<Point> allProfilePoints = new List<Point>();
            foreach (KeyValuePair<long, List<Point>> profilePoints in wellboreProfilePoints)
                allProfilePoints.AddRange(profilePoints.Value);

            if (allProfilePoints.Count == 0)
                return;

            InitializeDrawSetting(DrawSetting, drawSize, allProfilePoints);
            Image graphImage = GetGraphImage(DrawSetting);
            DrawingRange.Children.Add(graphImage);

            Image coordinateImage = GetCoordinatesImage(DrawSetting);
            DrawingRange.Children.Add(coordinateImage);

            foreach (KeyValuePair<long, List<Point>> profilePoints in wellboreProfilePoints)
            {
                Image profileImage = GetProfileImage(DrawSetting, profilePoints.Key, profilePoints.Value);
                DrawingRange.Children.Add(profileImage);
            }
        }

        private Image GetProfileImage(DrawSetting drawSetting, long id, List<Point> drawPoints)
        {
            Image image = new Image();
            image.Name = GetFullImageName(GetWellboreImageName(id));
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            DrawProfile(drawingContext, DrawSetting, drawPoints);

            drawingContext.Close();

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)drawSetting.DrawSize.Width, (int)drawSetting.DrawSize.Height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);
            bmp.Freeze();
            image.Source = bmp;
            return image;
        }

        private Image GetGraphImage(DrawSetting drawSetting)
        {
            Image image = new Image();
            image.Name = GetFullImageName(ImageNames.Graph);
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            DrawGrid(drawingContext, DrawSetting);

            drawingContext.Close();

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)drawSetting.DrawSize.Width, (int)drawSetting.DrawSize.Height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);
            bmp.Freeze();
            image.Source = bmp;
            return image;
        }

        private Image GetCoordinatesImage(DrawSetting drawSetting)
        {
            Image image = new Image();
            image.Name = GetFullImageName(ImageNames.Coordinates);
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            DrawCoordinates(drawingContext, DrawSetting);

            drawingContext.Close();

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)drawSetting.DrawSize.Width, (int)drawSetting.DrawSize.Height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);
            bmp.Freeze();
            image.Source = bmp;
            return image;
        }

        private string GetWellboreImageName(long id)
        {
            return String.Format("wllbore_{0}", id);
        }

        private string GetFullImageName(string name)
        {
            return String.Format("image_{0}", name);
        }

        private void InitializeDrawSetting(DrawSetting drawSetting, Size drawSize, List<Point> drawPoints)
        {
            Rect rectTrajectory = GetRectTrajectory(drawPoints);
            Rect marginsRectTrajectory = GetMarginsRectTrajectory(rectTrajectory, drawSetting);
            double scale = GetSacel(marginsRectTrajectory.Size, drawSize);
            Rect scaleRect = GetSacelRectTrajectory(marginsRectTrajectory, scale);
            double offsetX = drawSize.Width / 2 - scaleRect.Width / 2 - scaleRect.X;
            double offsetY = drawSize.Height / 2 - scaleRect.Height / 2 - scaleRect.Y;
            drawSetting.SetScale(scale);
            drawSetting.SetDarwSize(drawSize);
            drawSetting.SetBasePoint(new Point(offsetX, offsetY));
        }

        private void DrawGrid(DrawingContext drawingContext, DrawSetting drawSetting)
        {
            DrawHorizontalGridLines(drawingContext, drawSetting);
            DrawVerticalGridLines(drawingContext, drawSetting);
        }

        private void DrawHorizontalGridLines(DrawingContext drawingContext, DrawSetting drawSetting)
        {
            double scaleGraphStep = drawSetting.GetScaleGrapthStep();

            double stepCounterVerticalAxis12 = 0;
            for (double i = drawSetting.BasePoint.Y; i > 0; i -= scaleGraphStep)
            {
                drawingContext.DrawLine(drawSetting.GraphPen, new Point(0, i), new Point(drawSetting.DrawSize.Width, i));
                stepCounterVerticalAxis12 += drawSetting.GrapthStep;
            }

            int stepCounterVerticalAxis34 = 0;
            for (double i = drawSetting.BasePoint.Y; i < drawSetting.DrawSize.Height; i += scaleGraphStep)
            {
                drawingContext.DrawLine(drawSetting.GraphPen, new Point(0, i), new Point(drawSetting.DrawSize.Width, i));
                stepCounterVerticalAxis34 -= drawSetting.GrapthStep;
            }
        }

        private void DrawVerticalGridLines(DrawingContext drawingContext, DrawSetting drawSetting)
        {
            double scaleGraphStep = drawSetting.GetScaleGrapthStep();

            int stepCounterHorizontalAxis23 = 0;
            for (double i = drawSetting.BasePoint.X; i > 0; i = i - scaleGraphStep)
            {
                drawingContext.DrawLine(drawSetting.GraphPen, new Point(i, 0), new Point(i, drawSetting.DrawSize.Height));
                stepCounterHorizontalAxis23 += drawSetting.GrapthStep;
            }

            int stepCounterHorizontalAxis14 = 0;
            for (double i = drawSetting.BasePoint.X; i < drawSetting.DrawSize.Width; i += scaleGraphStep)
            {
                drawingContext.DrawLine(drawSetting.GraphPen, new Point(i, 0), new Point(i, drawSetting.DrawSize.Height));
                stepCounterHorizontalAxis14 += drawSetting.GrapthStep;
            }
        }

        private void DrawCoordinates(DrawingContext drawingContext, DrawSetting drawSetting)
        {
            DrawHorizontalCoordinatesLines(drawingContext, drawSetting);
            DrawVerticalCoordinatesLines(drawingContext, drawSetting);

            drawingContext.DrawLine(drawSetting.CoordinatesGraphPen, new Point(drawSetting.BasePoint.X, 0), new Point(drawSetting.BasePoint.X, drawSetting.DrawSize.Height));
            drawingContext.DrawLine(drawSetting.CoordinatesGraphPen, new Point(0, drawSetting.BasePoint.Y), new Point(drawSetting.DrawSize.Width, drawSetting.BasePoint.Y));
        }

        private void DrawHorizontalCoordinatesLines(DrawingContext drawingContext, DrawSetting drawSetting)
        {
            int coordinateStep = drawSetting.GetCoordinateStep();
            double scaleCoordinateStep = drawSetting.GetScaleCoordinateStep();

            double stepCounterVerticalAxis12 = 0;
            for (double i = drawSetting.BasePoint.Y; i > 0; i -= scaleCoordinateStep)
            {
                drawingContext.DrawLine(drawSetting.GraphBoltPen, new Point(0, i), new Point(drawSetting.DrawSize.Width, i));
                Point p1 = new Point(drawSetting.BasePoint.X, i);
                Point p2 = new Point(drawSetting.BasePoint.X + 10, i);
                drawingContext.DrawLine(drawSetting.CoordinatesGraphPen, p1, p2);
                drawingContext.DrawText(drawSetting.GetFormattedCoordinatesText(stepCounterVerticalAxis12.ToString()), p2);
                stepCounterVerticalAxis12 += coordinateStep;
            }

            int stepCounterVerticalAxis34 = 0;
            for (double i = drawSetting.BasePoint.Y; i < drawSetting.DrawSize.Height; i += scaleCoordinateStep)
            {
                drawingContext.DrawLine(drawSetting.GraphBoltPen, new Point(0, i), new Point(drawSetting.DrawSize.Width, i));
                Point p1 = new Point(drawSetting.BasePoint.X, i);
                Point p2 = new Point(drawSetting.BasePoint.X + 10, i);
                drawingContext.DrawLine(drawSetting.CoordinatesGraphPen, p1, p2);
                drawingContext.DrawText(drawSetting.GetFormattedCoordinatesText(stepCounterVerticalAxis34.ToString()), p2);
                stepCounterVerticalAxis34 -= coordinateStep;
            }
        }

        private void DrawVerticalCoordinatesLines(DrawingContext drawingContext, DrawSetting drawSetting)
        {
            int coordinateStep = drawSetting.GetCoordinateStep();
            double scaleCoordinateStep = drawSetting.GetScaleCoordinateStep();

            double stepCounterHorizontalAxis23 = 0;
            for (double i = drawSetting.BasePoint.X; i > 0; i -= scaleCoordinateStep)
            {
                drawingContext.DrawLine(drawSetting.GraphBoltPen, new Point(i, 0), new Point(i, drawSetting.DrawSize.Width));
                Point p1 = new Point(i, drawSetting.BasePoint.Y);
                Point p2 = new Point(i, drawSetting.BasePoint.Y + 10);
                drawingContext.DrawLine(drawSetting.CoordinatesGraphPen, p1, p2);
                drawingContext.DrawText(drawSetting.GetFormattedCoordinatesText(stepCounterHorizontalAxis23.ToString()), p2);
                stepCounterHorizontalAxis23 -= coordinateStep;
            }

            int stepCounterHorizontalAxis14 = 0;
            for (double i = drawSetting.BasePoint.X; i < drawSetting.DrawSize.Width; i += scaleCoordinateStep)
            {
                drawingContext.DrawLine(drawSetting.GraphBoltPen, new Point(i, 0), new Point(i, drawSetting.DrawSize.Width));
                Point p1 = new Point(i, drawSetting.BasePoint.Y);
                Point p2 = new Point(i, drawSetting.BasePoint.Y + 10);
                drawingContext.DrawLine(drawSetting.CoordinatesGraphPen, p1, p2);
                drawingContext.DrawText(drawSetting.GetFormattedCoordinatesText(stepCounterHorizontalAxis14.ToString()), p2);
                stepCounterHorizontalAxis14 += coordinateStep;
            }
        }

        private void DrawProfile(DrawingContext drawingContext, DrawSetting drawSetting, List<Point> drawPoints)
        {
            List<Point> scalePoints = GetScaleTrajectoryPoints(drawPoints, drawSetting.Scale);
            for (int i = 1; i < scalePoints.Count; i++)
            {
                Point previousPoint = scalePoints[i - 1];
                previousPoint.Offset(drawSetting.BasePoint.X, drawSetting.BasePoint.Y);
                Point currentPoint = scalePoints[i];
                currentPoint.Offset(drawSetting.BasePoint.X, drawSetting.BasePoint.Y);
                drawingContext.DrawLine(drawSetting.TrajectoryPen, previousPoint, currentPoint);
            }
        }

        private List<Point> GetScaleTrajectoryPoints(List<Point> points, double scale)
        {
            List<Point> scalePoints = new List<Point>();
            foreach (Point point in points)
            {
                scalePoints.Add(new Point(point.X * scale, point.Y * scale));
            }
            return scalePoints;
        }

        private Rect GetRectTrajectory(List<Point> profilePoint)
        {
            double minX = profilePoint.Min(i => i.X);
            double minY = profilePoint.Min(i => i.Y);
            double maxX = profilePoint.Max(i => i.X);
            double maxY = profilePoint.Max(i => i.Y);
            double rectWidth = Math.Abs(minX - maxX);
            double rectHeight = Math.Abs(minY - maxY);
            return new Rect(new Point(minX, minY), new Size(rectWidth, rectHeight));
        }

        private Rect GetMarginsRectTrajectory(Rect rectTrajectory, DrawSetting drawSetting)
        {
            return new Rect(new Point(rectTrajectory.X - drawSetting.Margins / 2, rectTrajectory.Y - drawSetting.Margins / 2),
                            new Size(rectTrajectory.Width + drawSetting.Margins, rectTrajectory.Height + drawSetting.Margins));
        }

        private Rect GetSacelRectTrajectory(Rect rect, double scale)
        {
            return new Rect(new Point(rect.X * scale, rect.Y * scale), new Size(rect.Width * scale, rect.Height * scale));
        }

        private double GetSacel(Size rectSize, Size drawSize)
        {
            double ratioWidth = drawSize.Width / rectSize.Width;
            double ratioHeight = drawSize.Height / rectSize.Height;
            return Math.Min(ratioWidth, ratioHeight);
        }
        
        private void RemoveAllChildren()
        {
            List<UIElement> childrens = new List<UIElement>();
            foreach (UIElement child in DrawingRange.Children)
                childrens.Add(child);

            foreach (UIElement child in childrens)
            {
                DrawingRange.Children.Remove(child);
            }
        }
    }
}