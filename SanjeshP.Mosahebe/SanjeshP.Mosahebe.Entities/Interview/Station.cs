using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Entities.StudentForm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Interview
{
    public class Station : BaseEntity<int>
    {
        [Required, MaxLength(200)]

        public string Name { get; set; }

        public ICollection<StationQuestion> StationQuestions { get; set; } = new List<StationQuestion>();
        public ICollection<FieldStation> FieldStations { get; set; } = new List<FieldStation>();

    }
}
