using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WellboreProfileView.Models.DataBaseModels
{
    [Table("WELLBORE")]
    public class Wellbore : Entity
    {
        [Column("WELL_ID")]
        public long WellId { get; set; }

        public List<ProfilePath> ProfilePaths { get; set; }

        public Wellbore()
        {
            ProfilePaths = new List<ProfilePath>();
        }
    }
}
