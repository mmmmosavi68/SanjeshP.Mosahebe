using System;

namespace SanjeshP.Mosahebe.Web.Areas.User.ViewModels.Menu
{
    #region MyRegion
    public class UserAccessMenusViewModel
    {
        public Guid id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public JStreeAttr a_attr { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public bool Person_Checkecd { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public bool Group_Checkecd { get; set; }

        public UserAccessMenusStateViewModel state { get; set; }
    }
    public class UserAccessMenusStateViewModel
    {
        public bool opened { get; set; }
        public bool selected { get; set; }
        public bool disabled { get; set; }

    }

    public class JStreeAttr
    {
        public string href { get; set; }
        public string title { get; set; }
    }
    #endregion Fore Tree view js
}
