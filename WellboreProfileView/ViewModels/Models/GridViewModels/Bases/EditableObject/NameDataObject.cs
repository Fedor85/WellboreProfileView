using System;

namespace WellboreProfileView.ViewModels
{
    public class NameDataObject : ICloneable
    {
        public string Name { get; set; }

        public object Clone()
        {
            return new NameDataObject() { Name = this.Name };
        }
    }
}