using System;

namespace SanjeshP.Mosahebe.Web.Areas.Menu.ViewModels.Code
{
    public class CodeTreeViewModel
    {
        public int id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public bool ShowCode { get; set; }
        public JStreeAttr a_attr { get; set; }
        public CodeTreeStateViewModel state { get; set; }
    }
    public class CodeTreeStateViewModel
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
}
