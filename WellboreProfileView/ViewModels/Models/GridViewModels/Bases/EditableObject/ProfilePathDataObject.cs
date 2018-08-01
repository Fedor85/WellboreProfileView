using System;

namespace WellboreProfileView.ViewModels
{
    public class ProfilePathDataObject : ICloneable
    {
        public double VerticalDepth { get; set; }

        public double InclinationAngle { get; set; }

        public double AzimuthAngle { get; set; }

        public double Extension { get; set; }

        public object Clone()
        {
           return new ProfilePathDataObject()
                      {
                          VerticalDepth = this.VerticalDepth,
                          InclinationAngle = this.InclinationAngle,
                          AzimuthAngle = this.AzimuthAngle,
                          Extension = this.Extension,
                      };
        }
    }
}