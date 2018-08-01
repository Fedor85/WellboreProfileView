using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;
using Utils;
using WellboreProfileView.Interfaces.Services;
using WellboreProfileView.Models;

namespace WellboreProfileView.Domain.Services
{
    public class CalculationTrajectoryService : ICalculationTrajectoryService
    {
        public List<Point3D> Get3DProfilePathPoints(List<ProfilePathPoint> profilePathPoints)
        {
            List<Point3D> point3Ds = new List<Point3D>();
            Point3D currentPoint3D = new Point3D();
            if (profilePathPoints.Count < 2)
                return point3Ds;

            point3Ds.Add(new Point3D(0, 0, 0));
            if (!MathHelper.IsMiscalculationEqual(profilePathPoints[0].VerticalDepth, 0))
                profilePathPoints.Insert(0, new ProfilePathPoint(0, 0, 0, 0));

            for (int i = 1; i < profilePathPoints.Count; i++)
            {
                ProfilePathPoint previousPoint = profilePathPoints[i - 1];
                ProfilePathPoint currentPoint = profilePathPoints[i];
                double mediumLenght = (currentPoint.GetLength() - previousPoint.GetLength()) / 2;
                double radI1 = MathHelper.AngleToRadian(previousPoint.InclinationAngle);
                double radA1 = MathHelper.AngleToRadian(previousPoint.AzimuthAngle);
                double radI2 = MathHelper.AngleToRadian(currentPoint.InclinationAngle);
                double radA2 = MathHelper.AngleToRadian(currentPoint.AzimuthAngle);

                double dogLegAngle = GetDogLegAngle(radI1, radI2, previousPoint.InclinationAngle, currentPoint.InclinationAngle, previousPoint.AzimuthAngle, currentPoint.AzimuthAngle);
                double ratioFactor = GetRatioFactor(dogLegAngle);

                Point3D point3D = new Point3D();
                point3D.X = currentPoint3D.X + GetXMinimumCurvatureMethod(mediumLenght, ratioFactor, radI1, radA1, radI2, radA2);
                point3D.Y = currentPoint3D.Y + GetYMinimumCurvatureMethod(mediumLenght, ratioFactor, radI1, radA1, radI2, radA2);
                point3D.Z = currentPoint3D.Z - GetZMinimumCurvatureMethod(mediumLenght, ratioFactor, radI1, radI2);
                point3Ds.Add(point3D);
                currentPoint3D = point3D;
            }
            return point3Ds;
        }

        public List<Point> GetHorizontalProjectionProfilePathPoints(List<ProfilePathPoint> profilePathPoints)
        {
            List<Point> points = new List<Point>();
            foreach (Point3D point3D in Get3DProfilePathPoints(profilePathPoints))
                points.Add(new Point(point3D.X, point3D.Y));

            return points;
        }

        public List<Point> GetVerticalProjectionProfilePathPoints(List<ProfilePathPoint> profilePathPoints)
        {
            List<Point> points = new List<Point>();
            foreach (Point3D point3D in Get3DProfilePathPoints(profilePathPoints))
                points.Add(new Point(point3D.X, point3D.Z));

            return points;
        }

        private static double GetDogLegAngle(double radI1, double radI2, double i1, double i2, double a1, double a2)
        {
            double cos = Math.Cos(MathHelper.AngleToRadian(i2 - i1)) - Math.Sin(radI1) * Math.Sin(radI2) * (1 - Math.Cos(MathHelper.AngleToRadian(a2 - a1)));
            return Math.Acos(cos);
        }

        private static double GetRatioFactor(double dogLegAngle)
        {
            if (MathHelper.IsMiscalculationEqual(dogLegAngle, 0))
                return 1;

            return 2 / dogLegAngle * Math.Tan(MathHelper.AngleToRadian(MathHelper.RadianToAngle(dogLegAngle) / 2));
        }

        private static double GetXMinimumCurvatureMethod(double mediumLenght, double rf, double radI1, double radA1, double radI2, double radA2)
        {
            return mediumLenght * (Math.Sin(radI1) * Math.Sin(radA1) + Math.Sin(radI2) * Math.Sin(radA2)) * rf;
        }

        private static double GetYMinimumCurvatureMethod(double mediumLenght, double rf, double radI1, double radA1, double radI2, double radA2)
        {
            return mediumLenght * (Math.Sin(radI1) * Math.Cos(radA1) + Math.Sin(radI2) * Math.Cos(radA2)) * rf;
        }

        private static double GetZMinimumCurvatureMethod(double mediumLenght, double rf, double radI1, double radI2)
        {
            return mediumLenght * (Math.Cos(radI1) + Math.Cos(radI2)) * rf;
        }
    }
}