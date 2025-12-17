using System.Collections.Generic;

namespace SanjeshP.Mosahebe.Common
{
    public class SearchPagingParameters
    {
        // فیلد عمومی جستجو (در صورت استفاده)
        public string SearchTerm { get; set; }

        // شماره صفحه فعلی
        public int PageIndex { get; set; } = 1;

        // تعداد آیتم‌ها در هر صفحه
        public int PageSize { get; set; } = 10;
        public string SortColumn { get; set; }
        public bool SortAscending { get; set; } = true;

        // فیلترهای اختصاصی برای هر ستون جدول
        public Dictionary<string, string> Filters { get; set; } = new();
    }
}
