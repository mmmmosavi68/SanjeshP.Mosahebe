using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Entities.StudentForm;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Interview
{
    public class Station : BaseEntity<int>
    {
        public string Name { get; set; }

        public ICollection<StationQuestion> StationQuestions { get; set; }
    }
}
