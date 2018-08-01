using System.Windows.Media.Media3D;

namespace Utils
{
    public static class MathHelper
    {
        public const double Miscaluclation = 0.0001;

        public static bool IsMiscalculationEqual(double value1, double value2)
        {
            return System.Math.Abs(value1 - value2) < Miscaluclation;
        }

        public static double AngleToRadian(double angle)
        {
            return angle * System.Math.PI / 180;
        }

        public static double RadianToAngle(double radian)
        {
            return radian * 180 / System.Math.PI;
        }

        public static Point3D GetMiddleLine(Point3D point1, Point3D point2)
        {
            double middleX = (point1.X + point2.X) / 2;
            double middleY = (point1.Y + point2.Y) / 2;
            double middleZ = (point1.Z + point2.Z) / 2;
            return new Point3D(middleX, middleY, middleZ);
        }
    }
}