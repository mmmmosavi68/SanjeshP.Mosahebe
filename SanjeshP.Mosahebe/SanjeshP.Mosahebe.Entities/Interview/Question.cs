using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Interview
{
    public class Question :BaseEntity<int>
    {
        public string Text { get; set; }
        public float MaxValue { get; set; }
    }
}
