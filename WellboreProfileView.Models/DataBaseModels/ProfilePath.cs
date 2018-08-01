using System.ComponentModel.DataAnnotations.Schema;
using WellboreProfileView.Models.Interfaces;

namespace WellboreProfileView.Models.DataBaseModels
{
    [Table("PROFILE_PATH")]
    public class ProfilePath : IIdEntity
    {
        public long Id { get; set; }

        [Column("WELLBORE_ID")]
        public long WellboreId { get; set; }

        [Column("VERTICAL_DEPTH")]
        public double VerticalDepth { get; set; }

        [Column("INCLINATION_ANGLE")]
        public double InclinationAngle { get; set; }

        [Column("AZIMUTH_ANGLE")]
        public double AzimuthAngle { get; set; }

        [Column("EXTENSION")]
        public double Extension { get; set; }
    }
}
