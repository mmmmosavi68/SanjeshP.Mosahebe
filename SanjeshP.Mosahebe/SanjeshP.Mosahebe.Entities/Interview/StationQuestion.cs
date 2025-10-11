using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Entities.StudentForm;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Interview
{
    public class StationQuestion :BaseEntity<int>
    {
        public string Text { get; set; }
        public float MaxValue { get; set; }

        public int StationId { get; set; }
        public Station Station { get; set; }
    }
}
