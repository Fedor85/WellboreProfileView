using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WellboreProfileView.Models.DataBaseModels
{
    [Table("WELL")]
    public class Well : Entity
    {
        [Column("AREA_ID")]
        public long AreaId { get; set; }

        public List<Wellbore> Wellbores { get; set; }

        public Well()
        {
            Wellbores = new List<Wellbore>();
        }
    }
}
