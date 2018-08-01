using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WellboreProfileView.Models.DataBaseModels
{
    [Table("AREA")]
    public class Area : Entity
    {
        public List<Well> Wells { get; set; }

        public Area()
        {
            Wells = new List<Well>();
        }
    }
}