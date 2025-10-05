using System.Collections.Generic;

namespace SanjeshP.Mosahebe.Web.Areas.DocumentVerification.ViewModels
{
    public class FormDataInputViewModel
    {
        public int FormSubmissionScoreId { get; set; }
        public int FormItemId { get; set; }

        // لیست مقادیر هر ردیف
        public List<SubmissionDataInputViewModel> Datas { get; set; } = new();
    }
}
