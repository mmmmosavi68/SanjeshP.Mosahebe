using System;
using System.Collections.Generic;

namespace SanjeshP.Mosahebe.Web.SharedViewModels
{
    public class PagedListAccessViewModel<T>
    {
        public List<ListItemAccessViewModel<T>> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortColumn { get; set; }
        public bool SortAscending { get; set; }
        public int StartRow => ((PageIndex - 1) * PageSize) + 1;
        public int EndRow => Math.Min(PageIndex * PageSize, TotalCount);
        public int PageCount => (int)Math.Ceiling((double)TotalCount / PageSize);

    }


    public class ListItemAccessViewModel<T>
    {
        public T Data { get; set; }  // مدل نمایشی مثل UserViewModel، RoleViewModel و ...

        // سطح دسترسی‌ها بر اساس اکشن/کنترلر/منطقه
        public Dictionary<string, bool> Permissions { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
