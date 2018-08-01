using Utils;

namespace WellboreProfileView.Models
{
    public class ProfilePathPoint
    {
        public double VerticalDepth { get; set; }

        public double InclinationAngle { get; set; }

        public double AzimuthAngle { get; set; }

        public double Extension { get; set; }

        public ProfilePathPoint()
        {
        }

        public ProfilePathPoint(double verticalDepth, double inclinationAngle, double azimuthAngle, double extension):this()
        {
            VerticalDepth = verticalDepth;
            InclinationAngle = inclinationAngle;
            AzimuthAngle = azimuthAngle;
            Extension = extension;
        }

        public double GetLength()
        {
            return VerticalDepth + Extension;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            ProfilePathPoint profilePathPoint = (ProfilePathPoint)obj;
            return MathHelper.IsMiscalculationEqual(VerticalDepth, profilePathPoint.VerticalDepth)
                   && MathHelper.IsMiscalculationEqual(InclinationAngle, profilePathPoint.InclinationAngle)
                   && MathHelper.IsMiscalculationEqual(AzimuthAngle, profilePathPoint.AzimuthAngle)
                   && MathHelper.IsMiscalculationEqual(Extension, profilePathPoint.Extension);
        }
    }
}