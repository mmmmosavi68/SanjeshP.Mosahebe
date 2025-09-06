using Newtonsoft.Json;
using SanjeshP.Mosahebe.WebFramework.Api;
using System;
using System.Collections.Generic;

namespace SanjeshP.Mosahebe.Web.Areas.User.ViewModels.Menu
{
    public class MenuTreeViewModel : BaseDto<MenuTreeViewModel, Entities.Menu.Menu, Guid>
    {
        public string Title { get; set; }
        public Guid? ParentId { get; set; }
        public int? PageCode { get; set; }
        public string Area { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool? ShowMenu { get; set; }
        public bool Person_Checkecd { get; set; }
        public bool Group_Checkecd { get; set; }
        public string CssClass { get; set; }
        public string Icon { get; set; }
        public string Link { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public virtual ICollection<MenuTreeViewModel> InverseParent { get; set; } = new List<MenuTreeViewModel>();
        public virtual MenuTreeViewModel Parent { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<MenuTreeViewModel> children { get; set; }

    }
}
