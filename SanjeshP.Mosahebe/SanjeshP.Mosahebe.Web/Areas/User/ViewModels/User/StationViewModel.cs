using SanjeshP.Mosahebe.Entities.Interview;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.User.ViewModels.User
{
    public class StationViewModel
    {
        public IEnumerable<Station> Stations  { get; set; }
        public string ActiveStation { get; set; }
        public Guid UserId  { get; set; }
    }
}
