using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WellboreProfileView
{
    public class DrawSetting
    {
        private Color graphColor;

        private SolidColorBrush graphSolidColorBrush;

        private Color coordinatesGraphColor;

        private SolidColorBrush coordinatesGraphSolidColorBrush;

        private Color trajectoryColor;

        private SolidColorBrush trajectorySolidColorBrush;

        private int coordinateStepAboutGraph;

        private Typeface textTypeface;

        public Pen GraphPen { get; private set; }

        public Pen GraphBoltPen { get; private set; }

        public Pen CoordinatesGraphPen { get; private set; }

        public Pen TrajectoryPen { get; private set; }

        public int GrapthStep { get; private set; }

        public Size DrawSize { get; private set; }

        public Point BasePoint { get; private set; }

        public double Scale { get; private set; }

        public double Margins { get; private set; }
     
        //3D

        public double Display3DDistanceFactor { get; private set; }

        public Color LightColor { get; private set; }

        public double Grid3DMianThickness { get; private set; }

        public double Grid3DThickness { get; private set; }

        public double Trajectory3DThickness { get; private set; }

        public double WindChartUpOffset { get; private set; }

        public double WindChartLengthLine { get; private set; }

        public double WindChartLengthCone { get; private set; }

        public double WindChartRadiusBaseCone { get; private set; }

        public int DrillUnitBaseHeight { get; private set; }

        public int DrillUnitUpRadius { get; private set; }

        public int DrillUnitBaseRadius { get; private set; }

        public int DrillUniHeight { get; private set; }

        public int DrillUniThickness { get; private set; }

        public DiffuseMaterial GridMainDiffuseMaterial { get; private set; }

        public DiffuseMaterial GridDiffuseMaterial { get; private set; }

        public DiffuseMaterial WindChartDiffuseMaterial { get; private set; }

        public DiffuseMaterial DrillUnitDiffuseMaterial { get; private set; }

        public DiffuseMaterial TrajectoryDiffuseMaterial { get; private set; }

        public DrawSetting()
        {
            graphColor = Color.FromArgb(255, 249, 218, 122);
            graphSolidColorBrush = new SolidColorBrush(graphColor);
            GraphPen = new Pen(graphSolidColorBrush, 1);
            GraphBoltPen = new Pen(graphSolidColorBrush, 1.5);

            coordinatesGraphColor = Color.FromArgb(255, 128, 128, 128);
            coordinatesGraphSolidColorBrush = new SolidColorBrush(coordinatesGraphColor);
            CoordinatesGraphPen = new Pen(coordinatesGraphSolidColorBrush, 1.5);

            trajectoryColor = Color.FromArgb(255, 153, 180, 209);
            trajectorySolidColorBrush = new SolidColorBrush(trajectoryColor);
            TrajectoryPen = new Pen(trajectorySolidColorBrush, 3);

            textTypeface = new Typeface(new FontFamily("Calibri"), FontStyles.Normal, FontWeights.Black, FontStretches.Normal);

            GrapthStep = 20;
            coordinateStepAboutGraph = 5;
            Margins = 100;
            Scale = 1;

            LightColor = Colors.White;

            Grid3DMianThickness = 7;
            Grid3DThickness = 3;
            Trajectory3DThickness = 20;

            GridMainDiffuseMaterial = new DiffuseMaterial(new SolidColorBrush(coordinatesGraphColor));
            GridMainDiffuseMaterial.Color = coordinatesGraphColor;

            GridDiffuseMaterial = new DiffuseMaterial(new SolidColorBrush(graphColor));

            WindChartLengthLine = 150;
            WindChartLengthCone = 40;
            WindChartRadiusBaseCone = 15;
            WindChartUpOffset = 50;

            Color windChartColor = Colors.Firebrick;
            WindChartDiffuseMaterial = new DiffuseMaterial(new SolidColorBrush(windChartColor));
            WindChartDiffuseMaterial.Color = windChartColor;

            Color drillUnitColor = Colors.Black;
            DrillUnitDiffuseMaterial = new DiffuseMaterial(new SolidColorBrush(drillUnitColor));
            DrillUnitDiffuseMaterial.Color = drillUnitColor;
            DrillUniThickness = 3;
            DrillUnitBaseHeight = 5;
            DrillUnitBaseRadius = 30;
            DrillUniHeight = 150;
            DrillUnitUpRadius = 15;

            TrajectoryDiffuseMaterial = new DiffuseMaterial(new SolidColorBrush(trajectoryColor));
            Display3DDistanceFactor = 1.5;
            Reset();
        }

        public FormattedText GetFormattedCoordinatesText(string text)
        {
            return new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, textTypeface, 10, coordinatesGraphSolidColorBrush);
        }

        public void SetScale(double scale)
        {
            Scale = scale;
        }

        public void SetBasePoint(Point basePoint)
        {
            BasePoint = basePoint;
        }

        public void SetDarwSize(Size drawSize)
        {
            DrawSize = drawSize;
        }

        public void Reset()
        {
            Scale = 1;
            DrawSize = Size.Empty;
            BasePoint = new Point();
        }

        public double GetScaleGrapthStep()
        {
            return GrapthStep * Scale;
        }

        public int GetCoordinateStep()
        {
            return GrapthStep * coordinateStepAboutGraph;
        }

        public double GetScaleCoordinateStep()
        {
            return GetCoordinateStep() * Scale;
        }
    }
}